using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.DTO;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.API.Controllers
{

	public class ProductsController : APIsBaseController
	{
		private readonly IGenericRepository<Product> _productRepo;
		private readonly IMapper _mapper;
		private readonly IGenericRepository<ProductType> _typeRepo;
		private readonly IGenericRepository<ProductBrand> _brandRepo;
		private readonly IUnitOfWork _unitOfWork;

		public ProductsController(IGenericRepository<Product> ProductRepo,
			IMapper mapper,
			IGenericRepository<ProductType> TypeRepo,
			IGenericRepository<ProductBrand> BrandRepo,
			IUnitOfWork unitOfWork)
		{
			_productRepo = ProductRepo;
			_mapper = mapper;
			_typeRepo = TypeRepo;
			_brandRepo = BrandRepo;
			_unitOfWork = unitOfWork;
		}

		// Get All Product
		[Authorize]
		[CachedAttribute(300)]
		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams Params)
		{
			var Spec = new ProductWithBrandAndTypeSpecictions(Params);
			var Products = await _unitOfWork.Repository<Product>().GetAllSpecAsync(Spec);
			var MappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);
			var CountSpec = new ProductWithFiltrationForCountAsync(Params);
			var Count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);
			var ReturnedObject = new Pagination<ProductToReturnDto>()
			{
				PageIndex = Params.PageSize,
				PageSize = Params.PageSize,
				Data = MappedProducts,
				Count = Count
			};
			
			
			
			return Ok(ReturnedObject);
		}
		// Get Product by id
		[HttpGet("{id}")]
		public async Task<ActionResult<Product>> GetProductByid(int id)
		{
			var Spec = new ProductWithBrandAndTypeSpecictions(id);
			var Product = await _productRepo.GetEntitySpecAsync(Spec);
			if (Product is null) return NotFound(new ApiResponse(400));
			var MappedProduct = _mapper.Map<Product, ProductToReturnDto>(Product);
			return Ok(MappedProduct);
		}

		[HttpGet("Types")]
		public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
		{
			var Types = await _typeRepo.GetAllAsync();
			return Ok(Types);
		}

		[HttpGet("Brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrand()
		{
			var Brands = await _brandRepo.GetAllAsync();
			return Ok(Brands);
		}



	}
}
