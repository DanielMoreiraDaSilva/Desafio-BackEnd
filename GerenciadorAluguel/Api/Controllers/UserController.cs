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
                var result = await _service.GetAllCNHTypeAsync(valid);
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
                var validate = await _service.ValidateNewUserAsync(user);

                if (!validate.Errors.Any())
                {
                    await _service.AddAsync(user);
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

        // No seu controller
        [HttpPost("atualizar-cadastro")]
        public async Task<IActionResult> AtualizarCadastro([FromServices] IUserService _service, [FromForm] IFormFile cnhImage)
        {
            if (cnhImage == null || cnhImage.Length == 0)
                return BadRequest("Nenhuma imagem da CNH foi enviada.");

            if (cnhImage.ContentType != "image/png" && cnhImage.ContentType != "image/bmp")
                return BadRequest("O formato do arquivo deve ser PNG ou BMP.");

            // Salvar a imagem no armazenamento escolhido (por exemplo, no disco local)
            string filePath = "caminho/do/arquivo/no/storage/" + cnhImage.FileName;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await cnhImage.CopyToAsync(stream);
            }

            // Atualizar o cadastro do entregador no banco de dados, armazenando apenas a referência para a foto (URL ou caminho do arquivo)
            string fotoUrl = "url/para/a/foto/no/storage";
            // Faça a atualização no banco de dados

            return Ok("Cadastro atualizado com sucesso.");
        }

    }
}
