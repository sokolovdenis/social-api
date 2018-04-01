using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Infrastructure.Swagger
{
	public class JsonOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			operation.Consumes.Clear();
			operation.Consumes.Add("application/json");

			operation.Produces.Clear();
			operation.Produces.Add("application/json");
		}
	}
}
