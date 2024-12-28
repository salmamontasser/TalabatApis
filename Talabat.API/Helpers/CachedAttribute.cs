using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.API.Helpers
{
	public class CachedAttribute : Attribute, IAsyncActionFilter
	{
		private readonly int _expireTimeInSeconds;

		public CachedAttribute(int ExpireTimeInSeconds)
        {
			_expireTimeInSeconds = ExpireTimeInSeconds;
		}
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var CacheService= context.HttpContext.RequestServices.GetRequiredService<IResponseCasheService>();
			var CacheKey = GenerateCachKeyFromRequest(context.HttpContext.Request);
			var CachedResponse = await CacheService.GetCachedResponse(CacheKey);
			if(!string.IsNullOrEmpty(CachedResponse))
			{
				var contentResult = new ContentResult()
				{
					Content = CachedResponse,
					ContentType="application/json",
					StatusCode=200
				};
				context.Result = contentResult;
			}

			var ExecutedEndPointContext = await next.Invoke();
			if(ExecutedEndPointContext.Result is OkObjectResult result)
			{
				await CacheService.CacheResponseAync(CacheKey, result.Value, TimeSpan.FromSeconds(_expireTimeInSeconds));
			}

		}

		private string GenerateCachKeyFromRequest(HttpRequest request)
		{
			var KeyBuilder = new StringBuilder();
			KeyBuilder.Append(request.Path);
            foreach (var (Key,value) in request.Query.OrderBy(X=>X.Key))
            {
				KeyBuilder.Append($"|{Key}-{value}");

            }
			return KeyBuilder.ToString();
        }
	}
}
