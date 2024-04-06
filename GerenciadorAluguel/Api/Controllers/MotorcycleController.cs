using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MotorcycleController : ControllerBase
    {   
        private readonly ILogger<MotorcycleController> _logger;

        public MotorcycleController(ILogger<MotorcycleController> logger)
        {
            _logger = logger;
        }

        [HttpPost("insertMotorcycle")]
        public async Task<IActionResult> Post([FromServices] IMotorcycleService _service, Motorcycle motorcycle)
        {
            try
            {
                var validate = await _service.ValidateUniquePlateAsync(motorcycle.Plate);

                if(validate)
                {
                    await _service.AddAsync(motorcycle);
                    return Ok("Motorcycle inserted");
                }
                else
                    return BadRequest($"Plate {motorcycle.Plate} already exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Doesn't possible insert motorcycle");
            }
        }

        [HttpGet("listMotorcycles")]
        public async Task<IActionResult> Get([FromServices] IMotorcycleService _service, [FromQuery] MotorcycleFilter? filter = null)
        {
            try
            {
                var result = await _service.GetAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Doesn't possible get motorcycle list");
            }

        }

        [HttpPut("updateMotorcycle")]
        public async Task<IActionResult> Put([FromServices] IMotorcycleService _service, [FromQuery] Guid idMotorcycle, [FromQuery] string plate)
        {
            try
            {
                var validate = await _service.ValidateUniquePlateAsync(plate);

                if (validate)
                {
                    await _service.UpdatePlateAsync(idMotorcycle, plate);
                    return Ok("Motorcycle plate updated");
                }
                else
                    return BadRequest($"Plate {plate} already exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Doesn't possible update plate of motorcycle");
            }
        }

        [HttpDelete("deleteMotorcycle")]
        public async Task<IActionResult> Put([FromServices] IMotorcycleService _service, [FromQuery] Guid idMotorcycle)
        {
            try
            {
                var validate = await _service.ValidateHireMotorcycleAsync(idMotorcycle);

                if (validate)
                {
                    await _service.DeleteAsync(idMotorcycle);
                    return Ok("Motorcycle deleted");
                }
                else
                    return BadRequest($"It wasn't possible exclude motorcycle. There is one or more hire for this motorcycle");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Doesn't possible delete this motorcycle");
            }
        }
    }
}