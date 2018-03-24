using System.Data;
using System.Threading.Tasks;
using Dapper;
using WebApi.Models;

namespace WebApi.DataSources
{
	public class MigrationDataSource : DataSource
	{
		public MigrationDataSource(Database databaseEngine) : base(databaseEngine) { }

		public async Task<Migration> Up(IDbConnection connection, IDbTransaction transaction)
		{
			return await connection.QuerySingleAsync<Migration>(
				"INSERT INTO [Migration] OUTPUT INSERTED.* DEFAULT VALUES;", null, transaction);
		}

		public async Task<Migration> GetLast()
		{
			Migration migration = null;
			await Database.ConnectAsync(async (connection) =>
			{
				migration = await connection.QuerySingleOrDefaultAsync<Migration>(
					"SELECT TOP 1 * FROM [Migration] ORDER BY Version DESC;");
			});
			return migration;
		}

		public async Task CreateIfNot()
		{
			await Database.ConnectAsync(async (connection) =>
			{
				await connection.ExecuteAsync($@"
					IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Migration'))
					BEGIN
						CREATE TABLE [Migration] (
							[Version] [INT] IDENTITY(1,1) NOT NULL,
							[Created] [DATETIME2](7) NOT NULL
						);
						ALTER TABLE [Migration]
						ADD CONSTRAINT [DF_Migration_Created]
						DEFAULT (SYSUTCDATETIME())
						FOR [Created];
					END
				");
			});
		}
	}
}
