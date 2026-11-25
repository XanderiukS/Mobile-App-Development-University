using Core.Interfaz;
using Microsoft.AspNetCore.Mvc;
using Modelo.Modelos;

namespace Ordenes_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenController : ControllerBase
    {
        private readonly IOrden _ordenServicio;

        public OrdenController(IOrden ordenServicio)
        {
            _ordenServicio = ordenServicio;
        }

        // ------------------------------------------------------------------
        // GET: api/Orden
        // Consulta todas las órdenes registradas.
        // ------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var ordenes = await _ordenServicio.ObtenerTodos();
            return Ok(ordenes);
        }

        // ------------------------------------------------------------------
        // GET: api/Orden/5
        // Consulta una orden por su Id de PostgreSQL.
        // ------------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var orden = await _ordenServicio.ObtenerPorId(id);
            if (orden == null)
            {
                return NotFound(new { mensaje = "Orden no encontrada." });
            }

            return Ok(orden);
        }

        // ------------------------------------------------------------------
        // POST: api/Orden
        // Registra una nueva orden:
        //   - la guarda en PostgreSQL
        //   - guarda un registro de bitácora en MongoDB
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Insertar([FromBody] MOrden orden)
        {
            if (orden == null)
            {
                return BadRequest(new { mensaje = "Los datos de la orden son obligatorios." });
            }

            if (string.IsNullOrWhiteSpace(orden.NumeroOrden))
            {
                return BadRequest(new { mensaje = "El número de orden es obligatorio." });
            }

            if (string.IsNullOrWhiteSpace(orden.NombreCliente))
            {
                return BadRequest(new { mensaje = "El nombre del cliente es obligatorio." });
            }

            if (string.IsNullOrWhiteSpace(orden.DireccionEntrega))
            {
                return BadRequest(new { mensaje = "La dirección de entrega es obligatoria." });
            }

            if (string.IsNullOrWhiteSpace(orden.UsuarioOperacion))
            {
                return BadRequest(new { mensaje = "El usuario que realiza la operación es obligatorio." });
            }

            if (orden.Monto <= 0)
            {
                return BadRequest(new { mensaje = "El monto debe ser mayor a cero." });
            }

            try
            {
                int idGenerado = await _ordenServicio.Insertar(orden);

                return Ok(new
                {
                    mensaje = "Orden registrada correctamente en PostgreSQL y en la bitácora de MongoDB.",
                    idOrden = idGenerado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al registrar la orden.",
                    error = ex.Message
                });
            }
        }

        // ------------------------------------------------------------------
        // PUT: api/Orden/5
        // Modifica una orden:
        //   - actualiza PostgreSQL
        //   - guarda un nuevo registro en MongoDB indicando el cambio y la fecha
        // ------------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] MOrden orden)
        {
            if (orden == null)
            {
                return BadRequest(new { mensaje = "Los datos de la orden son obligatorios." });
            }

            var existente = await _ordenServicio.ObtenerPorId(id);
            if (existente == null)
            {
                return NotFound(new { mensaje = "Orden no encontrada." });
            }

            // El Id lo manda la ruta, no el body.
            orden.IdOrden = id;

            try
            {
                bool resultado = await _ordenServicio.Actualizar(orden);
                if (!resultado)
                {
                    return StatusCode(500, new { mensaje = "No fue posible actualizar la orden." });
                }

                return Ok(new
                {
                    mensaje = "Orden actualizada correctamente. Cambio registrado en la bitácora de MongoDB."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar la orden.",
                    error = ex.Message
                });
            }
        }

        // ------------------------------------------------------------------
        // DELETE: api/Orden/5
        // Elimina una orden de PostgreSQL.
        // ------------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var existente = await _ordenServicio.ObtenerPorId(id);
            if (existente == null)
            {
                return NotFound(new { mensaje = "Orden no encontrada." });
            }

            bool resultado = await _ordenServicio.Eliminar(id);
            if (!resultado)
            {
                return StatusCode(500, new { mensaje = "No fue posible eliminar la orden." });
            }

            return Ok(new { mensaje = "Orden eliminada correctamente." });
        }
    }
}
