using Coree.Interfaz;
using Coree.Servicio;
using Microsoft.AspNetCore.Mvc;
using Modelo.Modelos;

namespace HotelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : Controller
    {
        private readonly IReserva _reserva;

        public ReservaController(IReserva reserva)
        {
            _reserva = reserva;
        }

        // GET: api/Reserva
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                return Ok(await _reserva.ObtenerTodas());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las reservas.", error = ex.Message });
            }
        }

        // POST: api/Reserva
        [HttpPost]
        public async Task<IActionResult> Insertar([FromBody] MReservaRequest request)
        {
            try
            {
                var reserva = await _reserva.Insertar(request);
                return Created($"api/Reserva/{reserva.IdReserva}", new
                {
                    mensaje = "Reserva registrada correctamente.",
                    reserva
                });
            }
            catch (Exception ex)
            {
                return Manejar(ex, "Error al registrar la reserva.");
            }
        }

        // PUT: api/Reserva/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] MReservaRequest request)
        {
            try
            {
                var reserva = await _reserva.Actualizar(id, request);
                return Ok(new { mensaje = "Reserva actualizada correctamente.", reserva });
            }
            catch (Exception ex)
            {
                return Manejar(ex, "Error al actualizar la reserva.");
            }
        }

        // DELETE: api/Reserva/5?usuario=admin
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, [FromQuery] string usuario)
        {
            try
            {
                await _reserva.Eliminar(id, usuario);
                return Ok(new { mensaje = "Reserva eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return Manejar(ex, "Error al eliminar la reserva.");
            }
        }

        private IActionResult Manejar(Exception ex, string mensajeGeneral)
        {
            return ex switch
            {
                ValidacionException => BadRequest(new { mensaje = ex.Message }),
                NoEncontradoException => NotFound(new { mensaje = ex.Message }),
                HabitacionNoDisponibleException => Conflict(new { mensaje = ex.Message }),
                _ => StatusCode(500, new { mensaje = mensajeGeneral, error = ex.Message })
            };
        }
    }
}
