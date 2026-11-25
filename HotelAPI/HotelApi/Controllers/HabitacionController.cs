using Coree.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionController : Controller
    {
        private readonly IHabitacion _habitacion;

        public HabitacionController(IHabitacion habitacion)
        {
            _habitacion = habitacion;
        }

        // GET: api/Habitacion
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                return Ok(await _habitacion.ObtenerTodas());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las habitaciones.", error = ex.Message });
            }
        }
    }
}
