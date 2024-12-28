﻿using Microsoft.Extensions.Configuration;
using Stripe;
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
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentService(IConfiguration configuration,
			IBasketRepository basketRepository,
			IUnitOfWork unitOfWork)
        {
			_configuration = configuration;
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
		}
        public async Task<CustomerBasket?> CreateOrUpdatePaymentntent(string BasketId)
		{
			StripeConfiguration.ApiKey = _configuration["StripeSettings:Secretkey"];

			var Basket =await _basketRepository.GetBasketAsync(BasketId);
			if (Basket is null) return null;
			var ShippingPrice = 0M;
			if (Basket.DeliveryMethodId.HasValue)
			{
				var DeliveryMethod =await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(Basket.DeliveryMethodId.Value);
				ShippingPrice=DeliveryMethod.Cost;
			}
			if(Basket.Items.Count > 0)
			{
                foreach (var item in Basket.Items)
                {
					var Product =await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
					if(item.Price != Product.Price)
						item.Price = Product.Price;
                }
            }
			var SubTotal = Basket.Items.Sum(item => item.Price * item.Quantity);

			var Service = new PaymentIntentService();

			PaymentIntent paymentIntent;
			if(string.IsNullOrEmpty(Basket.PaymentntentId)) //Create
			{
				var Options = new PaymentIntentCreateOptions()
				{
					Amount=(long) SubTotal * 100 +(long) ShippingPrice * 100,
					Currency="usd",
					PaymentMethodTypes = new List<string>() { "card"}
				};
				paymentIntent= await Service.CreateAsync(Options);
				Basket.PaymentntentId = paymentIntent.Id;
				Basket.ClientSecret = paymentIntent.ClientSecret;
			}
			else //Update
			{
				var Options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)SubTotal * 100 + (long)ShippingPrice * 100,

				};
				paymentIntent= await Service.UpdateAsync(Basket.PaymentntentId, Options);
				Basket.PaymentntentId= paymentIntent.Id;
				Basket.ClientSecret= paymentIntent.ClientSecret;
			}
			await _basketRepository.UpdateBasketAsync(Basket);
			return Basket;

		}

		public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string PaymentIntentId, bool flag)
		{
			var Spec= new OrderWithPaymentIntentIdSpec(PaymentIntentId);
			var Order = await _unitOfWork.Repository<Order>().GetEntitySpecAsync(Spec);
			if(flag)
			{
				Order.Status = OrderStatus.PaymentReceived;
			}
			else
			{
				Order.Status = OrderStatus.PaymentFailed;
			}
			_unitOfWork.Repository<Order>().Update(Order);
			await _unitOfWork.CompleteAsync();
			return Order;
		}
	}
}
