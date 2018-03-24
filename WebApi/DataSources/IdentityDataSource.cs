using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using WebApi.Models;

namespace WebApi.DataSources
{
	public class IdentityDataSource : DataSource
	{
		public IdentityDataSource(Database database) : base(database) { }

		public async Task<Identity> Create(Guid id, int userId, byte[] salt, byte[] hash)
		{
			Identity identity = null;
			await Database.ConnectAsync(async (connection) =>
			{
				identity = await connection.QuerySingleAsync<Identity>($@"
						INSERT INTO [Identity] ([Id], [UserId], [Salt], [Hash])
						OUTPUT INSERTED.*
						VALUES (@Id, @UserId, @Salt, @Hash);
					",
					new
					{
						Id = id,
						UserId = userId,
						Salt = salt,
						Hash = hash
					});
			});
			return identity;
		}

		public async Task<Identity> Read(Guid id)
		{
			Identity identity = null;
			await Database.ConnectAsync(async (connection) =>
			{
				identity = await connection.QuerySingleOrDefaultAsync<Identity>(
					"SELECT * FROM [Identity] WHERE [Id] = @Id;",
					new
					{
						Id = id
					});
			});
			return identity;
		}

		public static async Task Deploy_V01(IDbConnection connection, IDbTransaction transaction)
		{
			await connection.ExecuteAsync($@"
					CREATE TABLE [Identity] (
						[Id] [uniqueidentifier] NOT NULL,
						[UserId] [int] NOT NULL,
						[Salt] [binary](32) NOT NULL,
						[Hash] [binary](32) NOT NULL,
						CONSTRAINT [PK_Identity] PRIMARY KEY CLUSTERED ([Id] ASC),
						CONSTRAINT [FK_Identity_User] FOREIGN KEY ([UserId]) REFERENCES [User]([Id])
					);",
				null,
				transaction);
		}
	}
}
