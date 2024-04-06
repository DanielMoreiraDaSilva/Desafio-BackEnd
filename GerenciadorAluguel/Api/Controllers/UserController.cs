using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet("listCNHType")]
        public async Task<IActionResult> GetListCNH([FromServices] IUserService _service, [FromQuery] bool? valid = null)
        {
            try
            {
                var result = await _service.GetAllCNHTypeAsync(valid).ConfigureAwait(false);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Doesn't possible get CNH list");
            }

        }

        [HttpPost("insertUser")]
        public async Task<IActionResult> Post([FromServices] IUserService _service, User user)
        {
            try
            {
                var validate = await _service.ValidateNewUserAsync(user).ConfigureAwait(false);

                if (!validate.Errors.Any())
                {
                    await _service.AddAsync(user).ConfigureAwait(false);
                    return Ok("User inserted");
                }
                else
                    return BadRequest(validate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Doesn't possible insert motorcycle");
            }
        }

        [HttpPost("atualizar-cadastro")]
        public async Task<IActionResult> UpdaloadCnhImage([FromServices] IUserService _service, Guid idUser, IFormFile cnhImage)
        {
            try
            {
                if (cnhImage == null || cnhImage.Length == 0)
                    return BadRequest("Nenhuma imagem da CNH foi enviada.");

                if (cnhImage.ContentType != "image/png" && cnhImage.ContentType != "image/bmp")
                    return BadRequest("O formato do arquivo deve ser PNG ou BMP.");

                using var stream = new MemoryStream();
                await cnhImage.CopyToAsync(stream).ConfigureAwait(false);

                await _service.UpdaloadCnhImageAsync(idUser, stream, cnhImage.ContentType).ConfigureAwait(false);

                return Ok("Cadastro atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Doesn't possible update CNH image");
            }
        }

    }
}
