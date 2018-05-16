using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WebApi.Models;

namespace WebApi.DataSources
{
	public class PostDataSource : DataSource
	{
		public PostDataSource(Database database) : base(database) { }

		public async Task<IEnumerable<Post>> ReadWallAsync(int userId, int skip, int count)
		{
			IEnumerable<Post> posts = null;
			await Database.ConnectAsync(async (connection) =>
			{
				posts = await connection.QueryAsync<Post, User, Post>($@"
						SELECT * FROM [Post]
						INNER JOIN [User] ON [Post].[UserId] = [User].[Id]
						WHERE [Post].[UserId] = @UserId
						ORDER BY [Post].[DateTime] DESC
						OFFSET {skip} ROWS
						FETCH NEXT {count} ROWS ONLY;
					",
					(post, user) => 
					{
						post.User = user;
						return post;
					},
					new
					{
						UserId = userId
					});
			});
			return posts;
		}

		public async Task<IEnumerable<Post>> ReadFeedAsync(int userId, int skip, int count)
		{
			IEnumerable<Post> posts = null;
			await Database.ConnectAsync(async (connection) =>
			{
				posts = await connection.QueryAsync<Post, User, Post>($@"
						SELECT * FROM [Post]
						INNER JOIN [User] ON [Post].[UserId] = [User].[Id]
						WHERE [UserId] IN 
							(SELECT [FollowingId] FROM [Follow] WHERE [FollowerId] = @UserId)
						ORDER BY [DateTime] DESC
						OFFSET {skip} ROWS
						FETCH NEXT {count} ROWS ONLY;
					",
					(post, user) =>
					{
						post.User = user;
						return post;
					},
					new
					{
						UserId = userId
					});
			});
			return posts;
		}

		public async Task<Post> Create(int userId, string text)
		{
			IEnumerable<Post> posts = null;
			await Database.ConnectAsync(async (connection) =>
			{
				posts = await connection.QueryAsync<Post, User, Post>($@"
						INSERT INTO [Post] ([UserId], [Text])
						VALUES (@UserId, @Text);
						SELECT * FROM [Post] INNER JOIN [User] 
						ON [Post].[UserId] = [User].[Id]
						WHERE [Post].[Id] = SCOPE_IDENTITY();
					",
					(post, user) =>
					{
						post.User = user;
						return post;
					},
					new
					{
						UserId = userId,
						Text = text
					});
			});
			return posts.Single();
		}

		public async Task<Post> UpdateImage(int id, int userId, string imageUrl)
		{
			IEnumerable<Post> posts = null;
			await Database.ConnectAsync(async (connection) =>
			{
				posts = await connection.QueryAsync<Post, User, Post>($@"
						UPDATE [Post] SET [ImageUrl] = @ImageUrl
						WHERE [Id] = @Id AND [UserId] = @UserId AND [ImageUrl] IS NULL;
						SELECT * FROM [Post] INNER JOIN [User] 
						ON [Post].[UserId] = [User].[Id]
						WHERE [Post].[Id] = @Id;
					",
					(post, user) =>
					{
						post.User = user;
						return post;
					},
					new
					{
						Id = id,
						UserId = userId,
						ImageUrl = imageUrl
					});
			});
			return posts.Single();
		}

		public static async Task Deploy_V01(IDbConnection connection, IDbTransaction transaction)
		{
			await connection.ExecuteAsync($@"
					CREATE TABLE [Post] (
						[Id] [int] IDENTITY(1,1) NOT NULL,
						[UserId] [int] NOT NULL,
						[Text] [nvarchar](500) NOT NULL,
						[ImageUrl] [nvarchar](max) NULL,
						[DateTime] [datetime] NOT NULL DEFAULT (SYSUTCDATETIME()),
						CONSTRAINT [PK_Post] PRIMARY KEY NONCLUSTERED ([Id]),
						CONSTRAINT [FK_Post_User] FOREIGN KEY ([UserId]) REFERENCES [User]([Id]) ON DELETE CASCADE
					);
					CREATE CLUSTERED INDEX [IX_Post_UserId_DateTime] ON [Post] ([UserId] ASC, [DateTime] DESC);",
				null,
				transaction);
		}
	}
}
