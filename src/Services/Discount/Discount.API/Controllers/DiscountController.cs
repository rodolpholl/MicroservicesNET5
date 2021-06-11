using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _repository;
        private ILogger<DiscountController> _logger;

        public DiscountController(IDiscountRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }


        [HttpGet("{productName}", Name = "GetDiscount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Coupon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {

            try
            {

                var coupon = await _repository.GetDiscount(productName);
                return Ok(coupon);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during the Basket Search: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Coupon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
        {

            try
            {

                await _repository.CreateDiscount(coupon);
                return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during the Basket Search: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
        {

            try
            {
                return Ok(await _repository.UpdateDiscount(coupon));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during the Basket Search: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpDelete("{productName}", Name = "DeleteDiscount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Coupon>> DeleteDelete(string productName)
        {

            try
            {
                return Ok(await _repository.DeleteDiscount(productName));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during the Basket Search: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }


    }
}
