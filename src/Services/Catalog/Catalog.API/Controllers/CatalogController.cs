using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : Controller
    {
        private readonly IProductRepository _repository;
        private ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _repository.GetProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error Occurs.\n {ex.Message}");
                return NotFound();
            }

        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetProductById(string id)
        {
            try
            {
                var products = await _repository.GetProduct(id);
                return Ok(products);
            }
            catch
            {
                _logger.LogError($"Product with id {id} not found.");
                return NotFound();
            }

        }

        [HttpGet]
        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string categoryName)
        {
            try
            {
                var products = await _repository.GetProductByCategory(categoryName);
                return Ok(products);
            }
            catch
            {
                _logger.LogError($"Product with category {categoryName} not found.");
                return NotFound();
            }

        }

        [HttpGet]
        [Route("[action]/{category}", Name = "GetProductByName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name)
        {
            try
            {
                var products = await _repository.GetProductByName(name);
                return Ok(products);
            }
            catch
            {
                _logger.LogError($"Product with name {name} not found.");
                return NotFound();
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                await _repository.CreateProduct(product);
                return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error to create Product:\n {JsonSerializer.Serialize(product)}.\n {ex.Message}");
                return NotFound();
            }

        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> UpdateProduct([FromBody] Product product)
        {
            try
            {
                var result = await _repository.UpdateProduct(product);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on try to update Product with id {product.Id}:\n {JsonSerializer.Serialize(product)}.\n {ex.Message}");
                return NotFound();
            }

        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeleteProduct(string id)
        {
            try
            {
                var result = await _repository.DeleteProduct(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on try to delete Product with id {id}.\n {ex.Message}");
                return NotFound();
            }

        }

    }
}
