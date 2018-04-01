using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using WebApi.DataSources;
using WebApi.Infrastructure;
using WebApi.Infrastructure.Swagger;

namespace WebApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<JwtAuthenticationService.Options>(
				Configuration.GetSection("Authentication"));

			services.Configure<ImageProcessingService.Options>(
				Configuration.GetSection("ImageProcessing"));

			JwtAuthenticationService.AddJwtAuthentication(
				services,
				Configuration.GetValue<string>("Authentication:Secret"));

			services.AddSingleton(sp => 
				new Database(Configuration.GetConnectionString("DefaultConnection")));

			services.AddSingleton(sp =>
				new AzureStorage(Configuration.GetConnectionString("StorageConnection")));

			services.AddSingleton<MigrationDataSource>();
			services.AddSingleton<UserDataSource>();
			services.AddSingleton<IdentityDataSource>();
			services.AddSingleton<PostDataSource>();
			services.AddSingleton<FollowDataSource>();

			services.AddSingleton<MigrationService>();
			services.AddSingleton<IdentityService>();
			services.AddSingleton<ImageProcessingService>();
			services.AddSingleton<JwtAuthenticationService>();

			services.AddMvc(options =>
			{
				options.Filters.Add(new ValidateModelAttribute());
				options.InputFormatters.RemoveType<JsonPatchInputFormatter>();
			});

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Social API", Version = "v1" });
				c.IncludeXmlComments(Path.Combine(
					AppContext.BaseDirectory, 
					Assembly.GetEntryAssembly().GetName().Name + ".xml"));

				c.OperationFilter<JsonOperationFilter>();
				c.OperationFilter<AuthResponsesOperationFilter>();
				c.OperationFilter<FormFileOperationFilter>();
				c.OperationFilter<ResponseCodeOperationFilter>();
			});

			services.AddCors();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseAuthentication();

			app.UseCors(builder => builder
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
			);

			app.UseMvc();

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Social API V1");
				c.SupportedSubmitMethods(new SubmitMethod[0]); // disable Try button
			});

			InitializeDatabase(app);
		}

		private void InitializeDatabase(IApplicationBuilder app)
		{
			Database database = app.ApplicationServices.GetService<Database>();
			database.CreateIfNot().Wait();

			MigrationService migrationService = app.ApplicationServices.GetService<MigrationService>();
			migrationService.Deploy().Wait();
		}
	}
}
