using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Modelo.Modelos
{
    public class MReservaHistorialMongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int IdReserva { get; set; }
        public int NumeroHabitacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string TipoOperacion { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public DateTime FechaOperacion { get; set; }
    }
}
