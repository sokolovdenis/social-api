using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataSources;

namespace WebApi.Controllers
{
	[Route("migration")]
	public class ExampleController : Controller
	{
		public ExampleController()
		{
			_mds = new MigrationDataSource();
		}

		private readonly MigrationDataSource _mds;

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var migration = await _mds.GetLast();

			return Ok(migration);
		}

		[HttpPost]
		[Route("up")]
		public async Task<IActionResult> Up()
		{
			var migration = await _mds.Up();

			return Ok(migration);
		}

		[HttpPost]
		[Route("deploy")]
		public async Task<IActionResult> Deploy()
		{
			await _mds.DeployIfNot();

			return Ok();
		}
	}
}
