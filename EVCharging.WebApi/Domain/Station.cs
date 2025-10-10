using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EVCharging.WebApi.Domain
{
    public class Station
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = "";
        public string Type { get; set; } = "AC"; // AC | DC
        public int Slots { get; set; } // total concurrent slots
        public bool IsActive { get; set; } = true;

        public Geo Location { get; set; } = new();
        // Optional: lightweight availability model (time windows can be refined later)
        public List<StationSlotWindow> Availability { get; set; } = new();
    }

    public class Geo { public double Lat { get; set; } public double Lng { get; set; } }

    public class StationSlotWindow
    {
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int AvailableSlots { get; set; }
    }
}
