using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;

namespace WebApi.DataSources
{
	public class Database
	{
		public class Options
		{
			public string Host { get; set; }

			public string Name { get; set; }

			public string User { get; set; }

			public string Password { get; set; }

			public bool IsWindowsAuthentication =>
				string.IsNullOrEmpty(User) && string.IsNullOrEmpty(Password);
		}

		private readonly Options _options;
		private readonly string _connectionString;
		private readonly string _connectionStringNoDb;

		public Database(IOptions<Options> configurationOptions)
		{
			_options = configurationOptions.Value;

			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

			builder.DataSource = _options.Host;
			builder.IntegratedSecurity = _options.IsWindowsAuthentication;
			builder.UserID = _options.User;
			builder.Password = _options.Password;
			_connectionStringNoDb = builder.ToString();

			builder.InitialCatalog = _options.Name;
			_connectionString = builder.ToString();
		}

		public async Task ConnectAsync(Func<SqlConnection, Task> action)
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				await action(connection);
			}
		}

		public async Task ConnectWithTransactionAsync(Func<SqlConnection, SqlTransaction, Task> action)
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				using (SqlTransaction transaction = connection.BeginTransaction())
				{
					try
					{
						await action(connection, transaction);
						transaction.Commit();
					}
					catch (Exception e1)
					{
						try
						{
							transaction.Rollback();
						}
						catch (Exception e2)
						{
							throw new AggregateException(e1, e2);
						}
						throw;
					}
				}
			}
		}

		public async Task CreateIfNot()
		{
			using (SqlConnection connection = new SqlConnection(_connectionStringNoDb))
			{
				await connection.OpenAsync();
				await connection.ExecuteAsync(
					$@"
						IF (NOT EXISTS (SELECT * FROM master.dbo.sysdatabases WHERE [name] = '{_options.Name}'))
						BEGIN
							CREATE DATABASE [{_options.Name}];
						END
					");
			}
		}
	}
}
