using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using WebApi.Models;

namespace WebApi.DataSources
{
	public class MigrationDataSource
	{
		private static string connectionString = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Initial Catalog=ExampleDB;";

		public async Task<Migration> Up()
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				return await connection.QuerySingleAsync<Migration>("INSERT INTO [Migration] OUTPUT INSERTED.* DEFAULT VALUES;");
			}
		}

		public async Task<Migration> GetLast()
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				return await connection.QuerySingleOrDefaultAsync<Migration>("SELECT TOP 1 * FROM [Migration] ORDER BY Version DESC;");
			}
		}

		public async Task DeployIfNot()
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
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
			}
		}
	}
}
