using Modelo.Modelos;

namespace Coree.Interfaz
{
    public interface IHabitacion
    {
        Task<List<MHabitacion>> ObtenerTodas();
    }
}
