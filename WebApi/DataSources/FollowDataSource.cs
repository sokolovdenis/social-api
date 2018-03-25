using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using WebApi.Models;

namespace WebApi.DataSources
{
	public class FollowDataSource : DataSource
	{
		public FollowDataSource(Database database) : base(database) { }

		public async Task<IEnumerable<User>> ReadFollowers(int followingId)
		{
			IEnumerable<User> users = null;
			await Database.ConnectAsync(async (connection) =>
			{
				users = await connection.QueryAsync<User>($@"
						SELECT [User].* FROM [Follow]
						INNER JOIN [User] ON [Follow].[FollowerId] = [User].[Id]
						WHERE [Follow].[FollowingId] = @FollowingId;
					",
					new
					{
						FollowingId = followingId
					});
			});
			return users;
		}

		public async Task<IEnumerable<User>> ReadFollowings(int followerId)
		{
			IEnumerable<User> users = null;
			await Database.ConnectAsync(async (connection) =>
			{
				users = await connection.QueryAsync<User>($@"
						SELECT [User].* FROM [Follow]
						INNER JOIN [User] ON [Follow].[FollowingId] = [User].[Id]
						WHERE [Follow].[FollowerId] = @FollowerId;
					",
					new
					{
						FollowerId = followerId
					});
			});
			return users;
		}

		public async Task Create(int followerId, int followingId)
		{
			await Database.ConnectAsync(async (connection) =>
			{
				await connection.ExecuteAsync($@"
						INSERT INTO [Follow] ([FollowerId], [FollowingId])
						VALUES (@FollowerId, @FollowingId);
					",
					new
					{
						FollowerId = followerId,
						FollowingId = followingId
					});
			});
		}

		public async Task Delete(int followerId, int followingId)
		{
			await Database.ConnectAsync(async (connection) =>
			{
				await connection.ExecuteAsync($@"
						DELETE FROM [Follow]
						WHERE [FollowerId] = @FollowerId AND [FollowingId] = @FollowingId;
					",
					new
					{
						FollowerId = followerId,
						FollowingId = followingId
					});
			});
		}

		public static async Task Deploy_V01(IDbConnection connection, IDbTransaction transaction)
		{
			await connection.ExecuteAsync($@"
					CREATE TABLE [Follow] (
						[FollowerId] [int] NOT NULL,
						[FollowingId] [int] NOT NULL,
						[DateTime] [datetime] NOT NULL DEFAULT (SYSUTCDATETIME()),
						CONSTRAINT [PK_Follow] PRIMARY KEY CLUSTERED ([FollowerId], [FollowingId]),
						CONSTRAINT [FK_Follow_Follower] FOREIGN KEY ([FollowerId]) REFERENCES [User]([Id]),
						CONSTRAINT [FK_Follow_Following] FOREIGN KEY ([FollowingId]) REFERENCES [User]([Id])
					);
					CREATE INDEX [IX_Follow_Follower] ON [Follow] ([FollowerId]);
					CREATE INDEX [IX_Follow_Following] ON [Follow] ([FollowingId]);",
				null,
				transaction);
		}
	}
}
