
namespace Talabat.API.Errors
{
	public class ApiResponse
	{
        public int? StatusCode { get; set; }
		public string? Message { get; set; }

        public ApiResponse(int? statusCode,string? message = null)
        { 
            StatusCode = statusCode;
            Message = message?? GetDefaultMessageForstatusCode(statusCode);
        }

		private string? GetDefaultMessageForstatusCode(int? statusCode)
		{
			return StatusCode switch
			{
				400 => "Bad Request ",
				401 => "Unthorizored",
				404 => "Resource Not Found",
				500 => "Internal Server Error",
				_ => null
			};
		}
	}
}
