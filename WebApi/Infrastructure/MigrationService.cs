using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebApi.DataSources;
using WebApi.Models;

namespace WebApi.Infrastructure
{
	public class MigrationService
	{
		private readonly MigrationDataSource _migrationService;
		private readonly Database _dataSource;

		public MigrationService(MigrationDataSource mds, Database ds)
		{
			_migrationService = mds ?? throw new ArgumentNullException(nameof(mds));
			_dataSource = ds ?? throw new ArgumentNullException(nameof(ds));
		}

		public async Task Deploy()
		{
			await _migrationService.CreateIfNot();

			Migration migration = await _migrationService.GetLast();
			int version = migration?.Version ?? 0;

			for (var i = version; i < _deployProcList.Length; i++)
			{
				await _dataSource.ConnectWithTransactionAsync(async (connection, transaction) =>
				{
					await _deployProcList[i](connection, transaction);
					await _migrationService.Up(connection, transaction);
				});
			}
		}

		private readonly Func<SqlConnection, SqlTransaction, Task>[] _deployProcList
			= new Func<SqlConnection, SqlTransaction, Task>[]
		{
			async (connection, transaction) =>
			{
				await UserDataSource.Deploy_V01(connection, transaction);
				await IdentityDataSource.Deploy_V01(connection, transaction);
			}
		};
	}
}
