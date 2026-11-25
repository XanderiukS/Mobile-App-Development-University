using Modelo.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaz
{
    public interface IOrden
    {
        // GET  -> consultar todas las órdenes registradas.
        Task<List<MOrden>> ObtenerTodos();

        // GET  -> consultar una orden por su Id de PostgreSQL.
        Task<MOrden?> ObtenerPorId(int id);

        // POST -> registrar una orden en PostgreSQL + bitácora en MongoDB.
        Task<int> Insertar(MOrden orden);

        // PUT  -> modificar una orden en PostgreSQL + bitácora del cambio en MongoDB.
        Task<bool> Actualizar(MOrden orden);

        // DELETE -> eliminar una orden de PostgreSQL.
        Task<bool> Eliminar(int id);
    }
}
