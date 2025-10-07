using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EVCharging.WebApi.Domain
{
    public class Booking
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string OwnerNic { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;

        public DateTime ReservationStartUtc { get; set; }
        public DateTime ReservationEndUtc { get; set; }

        // Pending -> Approved -> Completed | Cancelled | Expired
        public string Status { get; set; } = "Pending";

        // Assigned upon approval
        public string? QrPayload { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
