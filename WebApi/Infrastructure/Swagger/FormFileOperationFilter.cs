using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Infrastructure.Swagger
{
	public class FormFileOperationFilter : IOperationFilter
	{
		private const string _formDataMimeType = "multipart/form-data";

		private static readonly string[] _formFilePropertyNames =
			typeof(IFormFile).GetTypeInfo().DeclaredProperties.Select(x => x.Name).ToArray();

		public void Apply(Operation operation, OperationFilterContext context)
		{
			if (context.ApiDescription.ParameterDescriptions.Any(x => x.ModelMetadata.ContainerType == typeof(IFormFile)))
			{
				NonBodyParameter[] formFileParameters = operation
					.Parameters
					.OfType<NonBodyParameter>()
					.Where(x => _formFilePropertyNames.Contains(x.Name))
					.ToArray();

				int parameterIndex = operation.Parameters.IndexOf(formFileParameters.First());

				foreach (NonBodyParameter formFileParameter in formFileParameters)
				{
					operation.Parameters.Remove(formFileParameter);
				}

				string formFileParameterName = context
					.ApiDescription
					.ActionDescriptor
					.Parameters
					.Where(x => x.ParameterType == typeof(IFormFile))
					.Select(x => x.Name)
					.First();

				NonBodyParameter parameter = new NonBodyParameter()
				{
					Name = formFileParameterName,
					In = "formData",
					Description = "The file to upload with \"multipart/form-data\" enctype form.",
					Required = true,
					Type = "file"
				};

				operation.Parameters.Insert(parameterIndex, parameter);

				if (!operation.Consumes.Contains(_formDataMimeType))
				{
					operation.Consumes.Add(_formDataMimeType);
				}
			}
		}
	}
}
