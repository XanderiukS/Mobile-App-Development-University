using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coree.Interfaz
{
    public interface IMovimiento
    {
        Task<int> Insertar(MMovimiento movimiento);
    }
}
