using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MotorcycleController : ControllerBase
    {
        private readonly ILogger<MotorcycleController> _logger;
        private readonly IMotorcycleService _service;

        public MotorcycleController(ILogger<MotorcycleController> logger, IMotorcycleService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("insertMotorcycle")]
        public async Task<IActionResult> PostInsertMotorcycle(Motorcycle motorcycle)
        {
            try
            {
                var isPlateUnique = await _service.IsPlateUniqueAsync(motorcycle.Plate);

                if (isPlateUnique)
                {
                    await _service.AddMotorcycleAsync(motorcycle);
                    _logger.LogInformation("Motorcycle inserted: {Plate}", motorcycle.Plate);
                    return Ok("Motorcycle inserted");
                }
                else
                {
                    _logger.LogWarning("Plate {Plate} already exists", motorcycle.Plate);
                    return BadRequest($"Plate {motorcycle.Plate} already exists");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert motorcycle");
                return BadRequest("Failed to insert motorcycle");
            }
        }

        [HttpGet("listMotorcycles")]
        public async Task<IActionResult> GetListMotorcycles([FromQuery] MotorcycleFilter filter)
        {
            try
            {
                var result = await _service.GetListMotorcycleAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get motorcycle list");
                return BadRequest("Failed to get motorcycle list");
            }
        }

        [HttpPut("updateMotorcycle")]
        public async Task<IActionResult> PutUpdateMotorcycle([FromQuery] Guid idMotorcycle, [FromQuery] string plate)
        {
            try
            {
                var isPlateUnique = await _service.IsPlateUniqueAsync(plate);

                if (isPlateUnique)
                {
                    await _service.UpdatePlateAsync(idMotorcycle, plate);
                    _logger.LogInformation("Motorcycle plate updated: {IdMotorcycle}, {Plate}", idMotorcycle, plate);
                    return Ok("Motorcycle plate updated");
                }
                else
                {
                    _logger.LogWarning("Plate {Plate} already exists", plate);
                    return BadRequest($"Plate {plate} already exists");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update plate of motorcycle");
                return BadRequest("Failed to update plate of motorcycle");
            }
        }

        [HttpDelete("deleteMotorcycle")]
        public async Task<IActionResult> DeleteMotorcycle([FromQuery] Guid idMotorcycle)
        {
            try
            {
                var hasRental = await _service.ThereIsRentalForMotorcycleAsync(idMotorcycle);

                if (hasRental)
                {
                    await _service.DeleteAsync(idMotorcycle);
                    _logger.LogInformation("Motorcycle deleted: {IdMotorcycle}", idMotorcycle);
                    return Ok("Motorcycle deleted");
                }
                else
                {
                    _logger.LogWarning("Could not delete motorcycle: there is one or more rental for this motorcycle");
                    return BadRequest("Could not delete motorcycle: there is one or more rental for this motorcycle");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete motorcycle");
                return BadRequest("Failed to delete motorcycle");
            }
        }

        [HttpGet("listRentalPlan")]
        public async Task<IActionResult> GetListRentalPlan()
        {
            try
            {
                var result = await _service.GetAllRentalPlansAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get rental plan list");
                return BadRequest("Failed to get rental plan list");
            }
        }

        [HttpPost("rental")]
        public async Task<IActionResult> PostRental(MotorcycleRentalPlan rental)
        {
            try
            {
                if(rental.ExpectedReturnDate.Value.Date <= DateTime.Today.Date)
                {
                    _logger.LogWarning("Expected return date most be bigger then today");
                    return BadRequest("Expected return date most be bigger then today");
                }

                var validate = await _service.ValidateNewMotorcycleRental(rental);

                if (!validate.Errors.Any())
                {
                    await _service.AddMotorcycleRentalAsync(rental);
                    _logger.LogInformation("Rental concluded");
                    return Ok("Rental concluded");
                }
                else
                {
                    _logger.LogWarning("Failed to insert motorcycle rental: {Errors}", string.Join(", ", validate.Errors));
                    return BadRequest(validate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert motorcycle rental");
                return BadRequest("Failed to insert motorcycle rental");
            }
        }

        [HttpGet("consultCost")]
        public async Task<IActionResult> ConsultCost([FromQuery] Guid idRentalPlan, [FromQuery] DateTime expectedReturnDate)
        {
            try
            {
                var result = await _service.CalculateCostPlan(idRentalPlan, expectedReturnDate);
                return Ok($"This plan will cost R$ {result}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to consult cost plan");
                return BadRequest("Failed to consult cost plan");
            }
        }
    }
}
