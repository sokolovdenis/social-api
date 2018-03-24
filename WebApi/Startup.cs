using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.DataSources;
using WebApi.Infrastructure;

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
			services.Configure<Database.Options>(
				Configuration.GetSection("DataSource"));

			services.Configure<JwtAuthenticationService.Options>(
				Configuration.GetSection("Authentication"));

			JwtAuthenticationService.AddJwtAuthentication(
				services,
				Configuration.GetValue<string>("Authentication:Secret"));

			services.AddSingleton<Database>();
			services.AddSingleton<MigrationDataSource>();
			services.AddSingleton<UserDataSource>();
			services.AddSingleton<IdentityDataSource>();
			services.AddSingleton<MigrationService>();
			services.AddSingleton<IdentityService>();
			services.AddSingleton<JwtAuthenticationService>();

			services.AddMvc(options =>
			{
				options.Filters.Add(new ValidateModelAttribute());
			});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseAuthentication();
			app.UseMvc();

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
