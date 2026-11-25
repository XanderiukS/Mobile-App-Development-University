namespace Modelo.Modelos
{
    public class MHabitacion
    {
        public int NumeroHabitacion { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal PrecioNoche { get; set; }
        public string Estado { get; set; } = "DISPONIBLE"; 
    }
}
