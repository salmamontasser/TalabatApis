using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.API.Extensions
{
	public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
		{

			Services.AddScoped<IBasketRepository, BasketRepository>();
			Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			Services.AddAutoMapper(typeof(MappingProfiles));
			Services.Configure<ApiBehaviorOptions>(Options =>
			{
				Options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
											  .SelectMany(P => P.Value.Errors)
											  .Select(E => E.ErrorMessage)
											  .ToList();
					var ValidationErrorsResponse = new ApiValidationError()
					{
						Errors = errors
					};
					return new BadRequestObjectResult(ValidationErrorsResponse);
				};
			});
			Services.AddScoped<IUnitOfWork,UnitOfWork> ();
			Services.AddScoped<IOrderService, OrderService>();
			Services.AddScoped<IPaymentService, PaymentService>();
			Services.AddSingleton<IResponseCasheService, CachedResponseService>();
			return Services; 
		}
	}
}
