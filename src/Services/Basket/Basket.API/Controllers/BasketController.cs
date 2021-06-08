using Basket.API.Entities;
using Basket.API.GRPCServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<BasketController> _logger;
        private readonly DiscountGRPCService _discountGrpcService;

        public BasketController(IBasketRepository repository, ILogger<BasketController> logger, DiscountGRPCService discountGrpcService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        }

        [HttpGet("{userName}",Name = "GetBasket")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShoppingCart))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            try
            {

                var basket = await _repository.GetBasket(userName);

                //Caso for a primeira consulta, retorna o primeiro objeto
                return Ok(basket ?? new ShoppingCart(userName));

            } catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during the Basket Search: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            try
            {
                // TODO : Communicate with Discount.GRPC
                // and Calculate lastest prices of product into shopping carto
                // consume Discount GRPC 

                foreach (var item in basket.Items)
                {
                    var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                    item.DiscountValue = coupon.Amount;
                }

                return Ok(await _repository.UpdateBasket(basket));
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during trying update Basket: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            try
            {
                await _repository.DeleteBasket(userName);
                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during trying delete Basket: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }
    }
}
