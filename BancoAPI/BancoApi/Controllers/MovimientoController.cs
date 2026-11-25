using Coree.Interfaz;
using Microsoft.AspNetCore.Mvc;
using Modelo.Modelos;

namespace BancoApi.Controllers
{
  
        [Route("api/[controller]")]
        [ApiController]
        public class MovimientoController : Controller
        {
            private readonly IMovimiento _movimiento;

            public MovimientoController(IMovimiento movimiento)
            {
                _movimiento = movimiento;
            }

            // POST: a  pi/Movimiento
            [HttpPost]
            public async Task<IActionResult> Insertar([FromBody] MMovimiento movimiento)
            {
                 try {
                    int idMovimiento = await _movimiento.Insertar(movimiento);

                    return Ok(new
                    {
                        mensaje = "Movimiento registrado correctamente.",
                        idMovimiento = idMovimiento
                    });
                }     
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        mensaje = "Error al registrar el movimiento.",
                        error = ex.Message
                    });
                }
            }


        }
    
}
