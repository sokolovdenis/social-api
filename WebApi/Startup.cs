using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = "yourdomain.com",
						ValidAudience = "yourdomain.com",
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes("У попа была собака."))
					};
				});

			services.Configure<Database.Options>(Configuration.GetSection("DataSource"));
			services.AddSingleton<Database>();
			services.AddSingleton<MigrationDataSource>();
			services.AddSingleton<UserDataSource>();
			services.AddSingleton<MigrationService>();

			services.AddMvc();
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

		private static void InitializeDatabase(IApplicationBuilder app)
		{
			Database database = app.ApplicationServices.GetService<Database>();
			database.CreateIfNot().Wait();

			MigrationService migrationService = app.ApplicationServices.GetService<MigrationService>();
			migrationService.Deploy().Wait();
		}
	}
}
