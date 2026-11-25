using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coree.Interfaz
{
    public interface ICliente
    {

        Task<List<MCliente>> ObtenerTodos();
        Task<MCliente?> ObtenerPorId(int id);
        Task<bool> Insertar(MCliente cliente);
        Task<bool> Actualizar(MCliente cliente);
        Task<bool> Eliminar(int id);
    }
}
