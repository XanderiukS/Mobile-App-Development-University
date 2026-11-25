using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Modelo.Modelos
{
    public class MOrdenMongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string NumeroOrden { get; set; } = string.Empty;
        public int IdOrdenPostgres { get; set; }

        public string Estado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
