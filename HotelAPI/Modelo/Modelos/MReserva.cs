namespace Modelo.Modelos
{
    public class MReserva
    {
        public int IdReserva { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int NumeroHabitacion { get; set; }
        public DateOnly FechaEntrada { get; set; }
        public DateOnly FechaSalida { get; set; }
        public int CantidadPersonas { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; } = "ACTIVA";
        public DateTime FechaRegistro { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }

    // Datos
    public class MReservaRequest
    {
        public string NombreCliente { get; set; } = string.Empty;
        public int NumeroHabitacion { get; set; }
        public DateOnly FechaEntrada { get; set; }
        public DateOnly FechaSalida { get; set; }
        public int CantidadPersonas { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }
}
