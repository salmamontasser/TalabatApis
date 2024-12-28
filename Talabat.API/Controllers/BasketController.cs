using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.DTO;
using Talabat.API.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.API.Controllers
{

	public class BasketController : APIsBaseController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;

		public BasketController(IBasketRepository basketRepository,
			IMapper mapper)
        {
			_basketRepository = basketRepository;
			_mapper = mapper;
		}

        // Get or ReCreate

        [HttpGet]
		public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string BasketId)
		{
			var Basket =await _basketRepository.GetBasketAsync(BasketId);
			return Basket is null ? new CustomerBasket(BasketId) : Basket;
		}

		// Update or Create New
		[HttpPost]
		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto Basket)
		{
			var MappedBasket=_mapper.Map<CustomerBasketDto,CustomerBasket>(Basket);
			var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(MappedBasket);
			if (CreatedOrUpdatedBasket is null) return BadRequest(new ApiResponse(400));
			return Ok(CreatedOrUpdatedBasket);
		}


		// Delete
		[HttpDelete]
		public async Task<ActionResult<bool>> DeleteBasket(string BasketId)
		{
			return await _basketRepository.DeleteBasketAsync(BasketId);
		}

	}
}
