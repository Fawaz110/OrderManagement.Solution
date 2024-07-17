using AutoMapper;
using Core.Entities;
using Core.Repositories.Contract;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Errors;
using Service.Dtos;

namespace OrderManagement.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _genericRepository;
        private readonly IMapper _mapper;

        public ProductsController(
            IGenericRepository<Product> genericRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        [HttpGet] // GET: /api/products
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var result = await _genericRepository.GetAllAsync();

            if (!result.Any())
                return NotFound(new ApiResponse(404));

            return Ok(result);
        }

        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{productId}")] // GET: /api/products/{productId}
        public async Task<ActionResult<Product>> GetById(int productId)
        {
            var product = await _genericRepository.GetByIdAsync(productId);

            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(product);
        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost] // POST: /api/products
        public async Task<ActionResult<ApiResponse>> AddProduct([FromBody] ProductDto model)
        {
            var product = _mapper.Map<Product>(model);

            await _genericRepository.AddAsync(product);

            var result = await _genericRepository.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400));

            return Ok(new ApiResponse(200));
        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPut("{productId}")] // PUT: /api/products/{productId}
        public async Task<ActionResult<ApiResponse>> UpdateProduct(int productId, [FromBody] ProductDto model)
        {
            var product = await _genericRepository.GetByIdAsync(productId);

            if (product is null)
                return BadRequest(new ApiResponse(400));

            product.Stock = model.Stock;
            product.Price = model.Price;
            product.Name = model.Name;

            _genericRepository.Update(product);

            var result = await _genericRepository.CompleteAsync();

            if (result <= 0)
                return (new ApiResponse(400, "error occured during update item"));

            return Ok(new ApiResponse(200));
        }
    }
}
