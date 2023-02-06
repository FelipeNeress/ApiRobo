using ApiRobo.Domain.Exceptions;
using ApiRobo.Domain.Models;
using ApiRobo.Service;
using Microsoft.AspNetCore.Mvc;

namespace ApiRobo.Controllers
{
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutoController(ProdutoService service)
        {
            _service = service;
        }

        [HttpGet("produto")]
        public IActionResult Listar([FromQuery] string? titulo)
        {
            return StatusCode(200, _service.Listar(titulo));
        }

        [HttpPost("produto")]
        public IActionResult Inserir()
        {
            try
            {
                _service.Inserir();
                return StatusCode(201);
            }
            catch (ValidacaoException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
