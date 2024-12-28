using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregation;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Specifiaction;

namespace Talabat.Service
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IPaymentService _paymentService;
		private readonly IUnitOfWork _unitOfWork;

		public OrderService(IBasketRepository basketRepository,
			IPaymentService paymentService,
			IUnitOfWork unitOfWork)
        {
			_basketRepository = basketRepository;
			_paymentService = paymentService;
			;
			_unitOfWork = unitOfWork;
		}
        public async Task<Order?> CreateOrderAsync(string BuyerEmail, string BasketId, int DeliveryMethodId, Address ShippingAddress)
		{
			//1)
			var Basket=await _basketRepository.GetBasketAsync(BasketId);
			//2)
			var OrderItems = new List<OrderItem>();
			if(Basket?.Items?.Count > 0)
			{
                foreach (var item in Basket.Items)
                {
					var Product =await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
					var ProductItemOrdered = new ProductItemOrder(Product.Id, Product.Name, Product.PictureUrl);
					var OrderItem = new OrderItem(ProductItemOrdered, item.Quantity,(int) Product.Price);
					OrderItems.Add(OrderItem);
                }
            }
			//3)
			var SubTotal=OrderItems.Sum(item=>item.Price * item.Quantity);
			//4)
			var DeliveryMethod =await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);
			//5)

			var Spec = new OrderWithPaymentIntentIdSpec(Basket.PaymentntentId);
			var ExOrder = await _unitOfWork.Repository<Order>().GetEntitySpecAsync(Spec);
			if(ExOrder is not null)
			{
				_unitOfWork.Repository<Order>().Delete(ExOrder);
				await _paymentService.CreateOrUpdatePaymentntent(BasketId);	
			}
			var Order = new Order(BuyerEmail, ShippingAddress, DeliveryMethod, OrderItems, SubTotal,Basket.PaymentntentId);
			//6)
			await _unitOfWork.Repository<Order>().AddAsync(Order);
			//7)
			
			var Result= await _unitOfWork.CompleteAsync();
			if (Result <= 0) return null;
			return Order;

		}

		public async Task<Order?> GetOrderByIdForSpecificUserAsync(string BuyerEmail, int OrderId)
		{
			var Spec=new OrderSpecification(BuyerEmail,OrderId);
			var Order =await _unitOfWork.Repository<Order>().GetEntitySpecAsync(Spec);
			return Order;
		}

		public async Task<IReadOnlyList<Order>> GetOrderForSpecificUserAsync(string BuyerEmail)
		{
			var Spec=new OrderSpecification(BuyerEmail);
			var Orders=await _unitOfWork.Repository<Order>().GetAllSpecAsync(Spec);
			return Orders;
		}
	}
}
