namespace Coree.Servicio
{
    public class ValidacionException : Exception
    {
        public ValidacionException(string mensaje) : base(mensaje) { }
    }
    public class NoEncontradoException : Exception
    {
        public NoEncontradoException(string mensaje) : base(mensaje) { }
    }
    public class HabitacionNoDisponibleException : Exception
    {
        public HabitacionNoDisponibleException(string mensaje) : base(mensaje) { }
    }
}
