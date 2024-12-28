using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.API.DTO
{
	public class OrderDto
	{
        [Required]
        public string BasketId { get; set; }
        [Required]
        public int DeliveryMethodId { get; set; }
        [Required]
        public AddressDto shipToAddress { get; set; }
    }
}
