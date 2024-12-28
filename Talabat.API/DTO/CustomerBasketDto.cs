using System.ComponentModel.DataAnnotations;

namespace Talabat.API.DTO
{
	public class CustomerBasketDto
	{
        [Required]
        public string Id { get; set; }


        public List<BasketItemDto> Items { get; set; }

		public string? PaymentntentId { get; set; }
		public string? ClientSecret { get; set; }

		public int? DeliveryMethodId { get; set; }
	}
}
