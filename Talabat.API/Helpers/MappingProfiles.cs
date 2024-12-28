using AutoMapper;
using Talabat.API.DTO;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.API.Helpers
{
	public class MappingProfiles : Profile
	{
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d=>d.ProductType,O=>O.MapFrom(S=>S.ProductType.Name))
				.ForMember(d => d.ProductBrand, O => O.MapFrom(S => S.ProductBrand.Name))
                .ForMember(d=>d.PictureUrl,O=>O.MapFrom<ProductPictureUrlResolver>( ));

            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();
            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto,BasketItem>();
            CreateMap<AddressDto, Core.Entities.Order_Aggregation.Address>()
				.ConstructUsing(dto => new Core.Entities.Order_Aggregation.Address(dto.FirstName, dto.LastName, dto.City, dto.Country, dto.Street)); ;
            CreateMap<Order, OrderToReturnDto>()
                .ForMember(D=>D.DeliveryMethod,O=>O.MapFrom(S=>S.DeliveryMethod.ShortName))
                .ForMember(D=>D.DeliveryMethod,O=>O.MapFrom(S=>S.DeliveryMethod.Cost));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
                .ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
                .ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
                .ForMember(d=>d.PictureUrl,O=>O.MapFrom<OrderItemPictureUrlResolver>());

		}
	}
}
