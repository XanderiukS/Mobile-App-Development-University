using System;

namespace Modelo.Modelos
{
    public class MOrden
    {
        public int IdOrden { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;
        public string DireccionEntrega { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string UsuarioOperacion { get; set; } = string.Empty;
    }
}
