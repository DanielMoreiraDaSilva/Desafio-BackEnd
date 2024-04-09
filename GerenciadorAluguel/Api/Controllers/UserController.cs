using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _service;

        public UserController(ILogger<UserController> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("listCNHType")]
        public async Task<IActionResult> GetListCNHType([FromQuery] bool? valid = null)
        {
            try
            {
                var result = await _service.GetAllCNHTypeAsync(valid).ConfigureAwait(false);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get CNH list");
                return BadRequest("Failed to get CNH list");
            }
        }

        [HttpGet("listUserNotifiedByDeliveryOrder")]
        public async Task<IActionResult> GetListUserNotifiedByDeliveryOrder([FromQuery] Guid idDeliveryOrder)
        {
            try
            {
                var result = await _service.GetListUserNotifiedByIdDeliveryOrder(idDeliveryOrder).ConfigureAwait(false);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user list notified by delivery order");
                return BadRequest("Failed to get user list notified by delivery order");
            }
        }

        [HttpPost("insertUser")]
        public async Task<IActionResult> PostInsertUser(User user)
        {
            try
            {
                var validate = await _service.ValidateNewUserAsync(user).ConfigureAwait(false);

                if (!validate.Errors.Any())
                {
                    await _service.AddAsync(user).ConfigureAwait(false);
                    _logger.LogInformation("User inserted: {UserId}", user.Id);
                    return Ok("User inserted");
                }
                else
                {
                    _logger.LogWarning("Failed to insert user: {Errors}", string.Join(", ", validate.Errors));
                    return BadRequest(validate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert user");
                return BadRequest("Failed to insert user");
            }
        }

        [HttpPost("updateCNHImage")]
        public async Task<IActionResult> UploadCnhImage([Required] Guid idUser, IFormFile cnhImage)
        {
            try
            {
                if (cnhImage == null || cnhImage.Length == 0)
                    return BadRequest("No CNH image was sent.");

                if (cnhImage.ContentType != "image/png" && cnhImage.ContentType != "image/bmp")
                    return BadRequest("The file format must be PNG or BMP.");

                using var stream = new MemoryStream();
                await cnhImage.CopyToAsync(stream).ConfigureAwait(false);

                await _service.UploadCnhImageAsync(idUser, stream, cnhImage.ContentType).ConfigureAwait(false);
                _logger.LogInformation("CNH image updated for user: {UserId}", idUser);

                return Ok("Registration updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update CNH image");
                return BadRequest("Failed to update CNH image");
            }
        }
    }
}
