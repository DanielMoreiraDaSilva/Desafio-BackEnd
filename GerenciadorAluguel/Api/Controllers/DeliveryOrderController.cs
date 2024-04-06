using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DeliveryOrderController : ControllerBase
    {
        private readonly ILogger<DeliveryOrderController> _logger;

        public DeliveryOrderController(ILogger<DeliveryOrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost("insertDeliveryOrder")]
        public async Task<IActionResult> Post([FromServices] IDeliveryOrderService _service, DeliveryOrder deliveryOrder)
        {
            try
            {
                var validate = await _service.IsStatusValidAsync(deliveryOrder.IdStatusDeliveryOrder);

                if (validate)
                {
                    await _service.AddDeliveyAsync(deliveryOrder);
                    return Ok("Delivery order inserted");
                }
                else
                    return BadRequest($"Id status delivery invalid");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Doesn't possible insert delivery order");
            }
        }

        [HttpGet("listStatusDelivery")]
        public async Task<IActionResult> GetStatusDelivery([FromServices] IDeliveryOrderService _service)
        {
            try
            {
                var result = await _service.GetAllStatusDeliveryAsync().ConfigureAwait(false);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Doesn't possible get status delivery list");
            }
        }
    }
}