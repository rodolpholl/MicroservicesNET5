using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrderList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator, ILogger<OrderController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{userName}", Name = "GetOrder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrdersVm>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrdersVm>>> GetOrderByUserName(string userName)
        {
            try
            {
                var query = new GetOrdersListQuery(userName);
                var orders = await _mediator.Send(query);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during searching for {userName} Orders: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }


        // testing porpouse
        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during the Order Checkout: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> UpdatetOrder([FromBody] UpdateOrderCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during update Order with Id {command.Id}: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpDelete("{id}",Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatetOrder(int id)
        {
            try
            {
                var command = new DeleteOrderCommand() { Id = id };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occurs during update Order with Id {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }
    }
}
