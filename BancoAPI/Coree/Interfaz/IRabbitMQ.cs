using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coree.Interfaz
{
    public interface IRabbitMQ
    {
        Task EncolarSolicitudPago(long idSolicitudPago);

    }
}
