using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EVCharging.WebApi.Domain
{
    public class User
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;   // for web & operator
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Backoffice";        // Backoffice | StationOperator
        public bool IsActive { get; set; } = true;
    }
}
