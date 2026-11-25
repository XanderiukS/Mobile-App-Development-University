using Coree.Interfaz;
using Microsoft.AspNetCore.Mvc;
using Modelo.Modelos;

namespace BancoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : Controller
    {
        private readonly ICliente _clienteServicio;

        public ClienteController(ICliente clienteServicio)
        {
            _clienteServicio = clienteServicio;
        }


        // GET: api/cliente/ObtenerTodos
        [HttpGet("ObtenerTodos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var clientes = await _clienteServicio.ObtenerTodos();

            return Ok(clientes);
        }


        // GET: api/cliente/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var cliente = await _clienteServicio.ObtenerPorId(id);
            if (cliente == null)
            {
                return NotFound(new {  mensaje = "Cliente no encontrado." });
            }

            return Ok(cliente);
        }

        // POST: api/cliente
        [HttpPost("Ingresar")]
        public async Task<IActionResult> Insertar([FromBody] MCliente cliente)
        {
            if (cliente == null)
            {
                return BadRequest(new { mensaje = "Los datos del cliente son obligatorios." });
            }

            if (string.IsNullOrWhiteSpace(cliente.Dpi))
            {
                return BadRequest(new
                { mensaje = "El DPI es obligatorio." });
            }

            if (string.IsNullOrWhiteSpace(cliente.Nombres))
            {
                return BadRequest(new
                { mensaje = "El Nombre es obligatorio." });
            }

            if (string.IsNullOrWhiteSpace(cliente.Apellidos))
            {
                return BadRequest(new
                { mensaje = "Los Apellidos son obligatorios." });
            }
            // faltan validaciones
            bool resultado = await _clienteServicio.Insertar(cliente);
            if (!resultado)
            {
                return StatusCode(500, new
                {   mensaje = "No fue posible registrar el cliente."  });
            }


            return Ok(new
            { mensaje = "Cliente registrado correctamente." });
        }


        // PUT: api/cliente/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id,   [FromBody] MCliente cliente)
        {
            if (cliente == null)
            {
                return BadRequest(new
                {   mensaje = "Los datos del cliente son obligatorios."  });
            }

            var clienteExistente = await _clienteServicio.ObtenerPorId(id);
            if (clienteExistente == null)
            {
                return NotFound(new    { mensaje = "Cliente no encontrado." });
            }

            cliente.IdCliente = id;

            bool resultado = await _clienteServicio.Actualizar(cliente);

            if (!resultado)
            {
                return StatusCode(500, new  { mensaje = "No fue posible actualizar el cliente." });
            }

            return Ok(new
            {
                mensaje = "Cliente actualizado correctamente."
            });

        }


        }
}
