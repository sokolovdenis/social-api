using System;
using System.Threading.Tasks;
using WebApi.DataSources;
using WebApi.Models;

namespace WebApi.Infrastructure
{
	public class MigrationService
	{
		public MigrationService(MigrationDataSource mds)
		{
			_mds = mds ?? throw new ArgumentNullException(nameof(mds));
		}

		public async Task Deploy()
		{
			// TODO: всё в транзакцию

			await _mds.DeployIfNot();

			Migration migration = await _mds.GetLast();
			int version = migration?.Version ?? 0;

			for (var i = version; i<_deployProcList.Length; i++)
			{
				await _deployProcList[i]();
				await _mds.Up();
			}
		}

		private readonly Func<Task>[] _deployProcList = new Func<Task>[]
		{
		};

		private readonly MigrationDataSource _mds;
	}
}
