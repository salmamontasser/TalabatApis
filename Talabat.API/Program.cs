using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.API.Errors;
using Talabat.API.Extensions;
using Talabat.API.Helpers;
using Talabat.API.MiddleWares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			#region Configure Services Add services to the container

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<StoreContext>(Options =>
			{
				Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});
			builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
			{
				Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));

			});



			builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
			{
				var Connection = builder.Configuration.GetConnectionString("RedisConnection");
				return ConnectionMultiplexer.Connect(Connection);
			});
			builder.Services.AddApplicationServices();


			builder.Services.AddIdentityServices(builder.Configuration);
			builder.Services.AddCors(Options=>
			{
				Options.AddPolicy("MyPolicy", options =>
				{
					options.AllowAnyHeader();
					options.AllowAnyMethod();
					options.WithOrigins(builder.Configuration["FrontBaseUrl"]);
				});
			});

			#endregion

			var app = builder.Build();



			#region Update-Database
			//StoreContext dbContext = new StoreContext();
			//await dbContext.Database.MigrateAsync();

			using var Scope = app.Services.CreateScope();

			var Services = Scope.ServiceProvider;

			var LoggerFactory =Services.GetRequiredService<ILoggerFactory>();
			try 
			{
				

				var dbContext = Services.GetRequiredService<StoreContext>();

				await dbContext.Database.MigrateAsync();

				var IdentityDbContext=Services.GetRequiredService<AppIdentityDbContext>();
				await IdentityDbContext.Database.MigrateAsync();

				var UserManager=Services.GetRequiredService<UserManager<AddUser>>();
				await AppIdentityDbContextSeed.SeedUserAsync(UserManager);


				await StoreContextSeed.SeedAsync(dbContext);
			}
			catch (Exception ex)
			{
				var Logger = LoggerFactory.CreateLogger<Program>();
				Logger.LogError(ex, "An Error Occured During Appling the Migration");
			}
			#endregion


			#region Configure - Configure the HTTP request pipeline
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMiddleware<ExceptionMiddleWare>();
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseStatusCodePagesWithReExecute("/errors/{0}");
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCors("MyPolicy");
			app.UseAuthentication();
			app.UseAuthorization();
			

			app.MapControllers(); 
			#endregion

			app.Run();
		}
	}
}
