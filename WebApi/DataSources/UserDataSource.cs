using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using WebApi.Models;

namespace WebApi.DataSources
{
	public class UserDataSource
	{
		private readonly string _connectionString;

		public UserDataSource(string connectionString)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
		}

		public async Task<User> Create(string name, string info, DateTime birthday)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QuerySingleAsync<User>($@"
						INSERT INTO [User] ([Name], [Info], [Birthday])
						OUTPUT INSERTED.*
						VALUES (@Name, @Info, @Birthday);
					",
					new
					{
						Name = name,
						Info = info,
						Birthday = birthday
					});
			}
		}

		public async Task<User> Read(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QuerySingleAsync<User>(
					"SELECT * FROM [User] WHERE [Id] = @Id;",
					new
					{
						Id = id
					});
			}
		}

		public async Task<IEnumerable<User>> Read()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QueryAsync<User>(
					"SELECT * FROM [User]");
			}
		}

		public async Task<User> Update(int id, string name, string info, DateTime birthday)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QuerySingleAsync<User>($@"
						UPDATE [User] SET [Name] = @Name, [Info] = @Info, [Birthday] = @Birthday
						OUTPUT INSERTED.*
						WHERE [Id] = @Id;
					",
					new
					{
						Id = id,
						Name = name,
						Info = info,
						Birthday = birthday
					});
			}
		}

		public async Task<User> Delete(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QuerySingleAsync<User>($@"
					DELETE FROM [User]
					OUTPUT DELETED.*
					WHERE [Id] = @Id;
				",
					new
					{
						Id = id
					});
			}
		}

		public async Task Deploy_V01()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.ExecuteAsync($@"
					CREATE TABLE [User](
						[Id] [int] IDENTITY(1,1) NOT NULL,
						[Name] [nvarchar](50) NOT NULL,
						[Info] [nvarchar](500) NOT NULL,
						[Birthday] [date] NOT NULL,
					 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
					(
						[Id] ASC
					);
				");
			}
		}
	}
}
