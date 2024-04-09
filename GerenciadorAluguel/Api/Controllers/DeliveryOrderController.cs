using Amazon.Runtime;
using Amazon.SQS;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class DeliveryOrderController : ControllerBase
    {
        private readonly ILogger<DeliveryOrderController> _logger;
        private readonly IDeliveryOrderService _service;

        public DeliveryOrderController(ILogger<DeliveryOrderController> logger, IDeliveryOrderService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("insert-delivery-order")]
        public async Task<IActionResult> PostInsertDeliveryOrder(DeliveryOrder deliveryOrder)
        {
            try
            {
                var isValidStatus = await _service.IsStatusValidAsync(deliveryOrder.IdStatusDeliveryOrder.Value);

                if (isValidStatus)
                {
                    await _service.AddDeliveryAsync(deliveryOrder);
                    _logger.LogInformation("Delivery order inserted: {Id}", deliveryOrder.Id);
                    return Ok("Delivery order inserted");
                }
                else
                {
                    _logger.LogWarning("Failed to insert delivery order due to invalid status: {IdStatusDeliveryOrder}", deliveryOrder.IdStatusDeliveryOrder);
                    return BadRequest($"Invalid status for delivery order");
                }
            }
            catch (AmazonSQSException ex)
            {
                _logger.LogError(ex, "Failed to send notification delivery order, but the delivery order inserted");
                return BadRequest("Failed to send notification delivery order, but the delivery order inserted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert delivery order");
                return BadRequest("Failed to insert delivery order");
            }
        }

        [HttpPost("accept-delivery-order")]
        public async Task<IActionResult> PostAcceptDeliveryOrder(Guid idUser, Guid idDeliveryOrder)
        {
            try
            {
                var isUserNotifiedValid = await _service.IsUserNotifiedValidAsync(idUser, idDeliveryOrder);

                if (isUserNotifiedValid)
                {
                    await _service.AcceptDeliveryOrderByUser(idUser, idDeliveryOrder);
                    _logger.LogInformation("Delivery order accepted by user: {IdUser}, DeliveryOrderId: {IdDeliveryOrder}", idUser, idDeliveryOrder);
                    return Ok("Delivery order accepted");
                }
                else
                {
                    _logger.LogWarning("User {IdUser} attempted to accept order {IdDeliveryOrder} without notification", idUser, idDeliveryOrder);
                    return BadRequest($"User cannot accept order without notification");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to accept delivery order");
                return BadRequest("Failed to accept delivery order");
            }
        }

        [HttpPost("conclude-delivery-order")]
        public async Task<IActionResult> PostConcludeDeliveryOrder(Guid idDeliveryOrder)
        {
            try
            {
                await _service.ConcludeDeliveryOrderByUser(idDeliveryOrder);
                _logger.LogInformation("Delivery order concluded: {IdDeliveryOrder}", idDeliveryOrder);
                return Ok("Delivery order concluded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to conclude delivery order");
                return BadRequest("Failed to conclude delivery order");
            }
        }

        [HttpGet("list-status-delivery")]
        public async Task<IActionResult> GetStatusDelivery()
        {
            try
            {
                var result = await _service.GetAllStatusDeliveryAsync().ConfigureAwait(false);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get status delivery list");
                return BadRequest("Failed to get status delivery list");
            }
        }
    }
}
