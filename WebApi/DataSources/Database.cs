using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace WebApi.DataSources
{
	public class Database
	{
		private readonly string _connectionString;
		private readonly string _connectionStringNoDb;
		private readonly string _name;

		public Database(string connectionString)
		{
			_connectionString = connectionString;

			SqlConnectionStringBuilder builder =
				new SqlConnectionStringBuilder(connectionString);
			_name = builder.InitialCatalog;

			builder.InitialCatalog = "";
			_connectionStringNoDb = builder.ToString();
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
						IF (NOT EXISTS (SELECT * FROM master.dbo.sysdatabases WHERE [name] = '{_name}'))
						BEGIN
							CREATE DATABASE [{_name}];
						END
					");
			}
		}
	}
}
