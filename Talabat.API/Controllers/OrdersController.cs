using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.API.DTO;
using Talabat.API.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order_Aggregation;
using Talabat.Core.Services;

namespace Talabat.API.Controllers
{

	public class OrdersController : APIsBaseController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public OrdersController(IOrderService orderService,IMapper mapper
			,IUnitOfWork unitOfWork)
        {
			_orderService = orderService;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}
		// Create
		[Authorize]
		[HttpPost]
		public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
		{
			var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var MappedAddress = _mapper.Map<AddressDto, Address>(orderDto.shipToAddress);
			var Order = await _orderService.CreateOrderAsync(BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, MappedAddress);
		    if(Order is null) return BadRequest(new ApiResponse(404, "There is a Problem With Your Order"));
			return Ok(Order);

		}
		[Authorize]
		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
		{
			var BuyerEmail=User.FindFirstValue(ClaimTypes.Email);
			var Orders=await _orderService.GetOrderForSpecificUserAsync(BuyerEmail);
			if (Orders is null) return NotFound(new ApiResponse(404, "There is no orders for this user"));
			var MappedOrders = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(Orders);
			return Ok(MappedOrders);
		}

		[Authorize]
		[HttpGet("{id}")]
		public async Task<ActionResult<Order>> GetOrderBtIdForUser(int id)
		{
			var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var order= await _orderService.GetOrderByIdForSpecificUserAsync(BuyerEmail,id);
			if (order is null) return NotFound(new ApiResponse(404, $"There is no order with {id} for this user"));
			var MappedOrder = _mapper.Map<Order, OrderToReturnDto>(order);
			return Ok(MappedOrder);
		}

		[HttpGet("DeliveryMethods")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
		{
			var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
			return Ok(DeliveryMethods);
		}
	}
}
