using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using WebApi.Models;

namespace WebApi.DataSources
{
	public class UserDataSource : DataSource
	{
		public UserDataSource(Database database) : base(database) { }

		public async Task<User> Create(string name, string info, DateTime birthday)
		{
			User user = null;
			await Database.ConnectAsync(async (connection) =>
			{
				user = await connection.QuerySingleAsync<User>($@"
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
			});
			return user;
		}

		public async Task<User> Read(int id)
		{
			User user = null;
			await Database.ConnectAsync(async (connection) =>
			{
				user = await connection.QuerySingleAsync<User>(
					"SELECT * FROM [User] WHERE [Id] = @Id;",
					new
					{
						Id = id
					});
			});
			return user;
		}

		public async Task<IEnumerable<User>> Read()
		{
			IEnumerable<User> users = null;
			await Database.ConnectAsync(async (connection) =>
			{
				users = await connection.QueryAsync<User>(
					"SELECT * FROM [User]");
			});
			return users;
		}

		public async Task<User> Update(int id, string name, string info, DateTime birthday)
		{
			User user = null;
			await Database.ConnectAsync(async (connection) =>
			{
				user = await connection.QuerySingleAsync<User>($@"
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
			});
			return user;
		}

		public async Task<User> Delete(int id)
		{
			User user = null;
			await Database.ConnectAsync(async (connection) =>
			{
				// Deleting from [Follow] table is required because SQL Server does not support multiple cascade paths.
				user = await connection.QuerySingleAsync<User>($@"
						DELETE FROM [User]
						OUTPUT DELETED.*
						WHERE [Id] = @Id;
						DELETE FROM [Follow] WHERE [FollowerId] = @Id OR [FollowingId] = @Id;
					",
					new
					{
						Id = id
					});
			});
			return user;
		}

		public static async Task Deploy_V01(IDbConnection connection, IDbTransaction transaction)
		{
			await connection.ExecuteAsync($@"
					CREATE TABLE [User] (
						[Id] [int] IDENTITY(1,1) NOT NULL,
						[Name] [nvarchar](50) NOT NULL,
						[Info] [nvarchar](500) NOT NULL,
						[Birthday] [date] NOT NULL,
						CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC)
					);",
				null,
				transaction);
		}
	}
}
