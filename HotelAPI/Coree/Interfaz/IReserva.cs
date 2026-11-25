using Modelo.Modelos;

namespace Coree.Interfaz
{
    public interface IReserva
    {
        Task<List<MReserva>> ObtenerTodas();
        Task<MReserva> Insertar(MReservaRequest reserva);
        Task<MReserva> Actualizar(int idReserva, MReservaRequest reserva);
        Task Eliminar(int idReserva, string usuario);
    }
}
