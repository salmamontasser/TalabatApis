using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Stripe;
using Talabat.API.DTO;
using Talabat.API.Errors;
using Talabat.Core.Services;

namespace Talabat.API.Controllers
{

	public class PaymentsController : APIsBaseController
	{
		private readonly IPaymentService _paymentService;
		const string endpointSecret = "whsec_43f5cdd277180a45808109a0076866d372071c1ec77190588d3843eb0d006777";
		public PaymentsController(IPaymentService paymentService)
        {
			_paymentService = paymentService;
		}
		[Authorize]
        [HttpPost("{basketId}")]
		public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdate(string basketId)
		{
			var Basket =await _paymentService.CreateOrUpdatePaymentntent(basketId);
			if (Basket is null) return BadRequest(new ApiResponse(400));
			return Ok(Basket);
		}
		[HttpPost("webhook")]
		public async Task<IActionResult> StripeWebHook() 
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], endpointSecret);
				var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;
				if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
				{
					await _paymentService.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id,false);
				}
				else if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
				{
					await _paymentService.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, true);
				}
				else
				{
					Console.WriteLine("Unhandled event type:{0}", stripeEvent.Type);
				}
				return Ok();

			}
			catch (StripeException ex)
			{ 
				return BadRequest(ex);
			}
		}
	}
}
 