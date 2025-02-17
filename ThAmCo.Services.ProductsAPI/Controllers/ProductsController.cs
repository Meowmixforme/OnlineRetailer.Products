using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using ThAmCo.ProductsAPI.Data;

namespace ThAmCo.ProductsAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class ProductsController : ControllerBase
    {
        private readonly ProductsContext _context;

        private readonly ILogger<ProductsController> _logger;


        public ProductsController(ProductsContext context, ILogger<ProductsController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        // GET: api/Products
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            try
            {
                //project into the DTO shape
                var Product = await _context
                    .Products
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Description = p.Description,

                    }).ToListAsync();

                if (Product == null || Product.Count == 0)
                {
                    return NotFound();
                }
                //return them
                return Ok(Product);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                // Return error 500 if not found
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            try
            {
                var product = await _context
                .Products
                .FirstOrDefaultAsync(m => m.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                // Project them into the DTO shape
                ProductDTO dto = new ProductDTO();
                dto.Id = id;
                dto.Name = product.Name;
                dto.Price = product.Price;
                dto.Description = product.Description;

                // Return the DTO
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                // Return error response 500
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductDTO>> PutProduct(int id, ProductDTO productDto)
        {
            try
            {
                if (id != productDto.Id)
                {
                    return BadRequest();
                }

                var product = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);

                if (product == null)
                {
                    return NotFound(); // Or another appropriate status code
                }

                // Project them into the entity to serve from DTO
                product.Id = productDto.Id;
                product.Name = productDto.Name;
                product.Price = productDto.Price;
                product.Description = productDto.Description;


                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                // Return error response 500
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductDTO productDto)
        {
            try
            {
                // Project them into the entity to serve from DTO
                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                Console.Error.WriteLine($"DbUpdateException error details - {innerExceptionMessage}");
                return StatusCode(500, $"A database error occoured: {innerExceptionMessage}");
            }
            catch (Exception ex)
            {
                {
                    // Added innerExceptionMessage logging to fix an issue
                    var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                    Console.Error.WriteLine($"Internal server eoor: {innerExceptionMessage}");
                    return StatusCode(500, $"An error occoured: {innerExceptionMessage}");
                    //_logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                    //// Return error response 500
                    //return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }


        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductDTO>> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                // // Return error response 500
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool ProductExists(int id)
        {
            try
            {
                return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
            }
            catch (Exception)
            {
                // Return false if no customer id
                return false;
            }
        }


    }

}