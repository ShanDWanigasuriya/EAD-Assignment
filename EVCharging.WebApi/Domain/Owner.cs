using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EVCharging.WebApi.Domain
{
    public class Owner
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string NIC { get; set; } = string.Empty;   // unique logical key
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public bool IsActive { get; set; } = true;        // deactivate self; reactivate by Backoffice
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
