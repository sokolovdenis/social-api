using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Infrastructure.Swagger
{
	public class ResponseCodeOperationFilter : IOperationFilter
	{
		private readonly Dictionary<string, string> _codes = new Dictionary<string, string>()
		{
			{ "400", "If some request parameters are not valid." },
			{ "401", "If Bearer token is not provided in request headers. Unable to authenticate." },
			{ "404", "If some record is missing in database. Check request parameters." },
			{ "409", "If some record is already exists in database. Check request parameters." },
			{ "500", "If server error occured. Please, contact REST API developer." }
		};

		public void Apply(Operation operation, OperationFilterContext context)
		{
			foreach(var code in _codes)
			{
				bool removed = operation.Responses.Remove(code.Key);
				if (removed)
				{
					operation.Responses.Add(code.Key, new Response()
					{
						Description = code.Value
					});
				}
			}
		}
	}
}
