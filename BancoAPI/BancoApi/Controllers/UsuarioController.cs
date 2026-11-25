using Coree.Interfaz;
using Microsoft.AspNetCore.Mvc;
using Modelo.Modelos;

namespace BancoApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UsuarioController : Controller
    {

        private readonly IConfiguration _config;

        private readonly IUsuario _usuarioServicio;

        public UsuarioController(IConfiguration config, IUsuario usuarioServicio)
        {
            _config = config;
            _usuarioServicio = usuarioServicio;
        }

        [HttpGet("ObtenerTodos")]
        public async Task<ActionResult<List<MUsuario>>> ObtenerTodos()
        {
            var usuarios = await _usuarioServicio.ObtenerTodos();

            return Ok(usuarios);
        }


    }
}
