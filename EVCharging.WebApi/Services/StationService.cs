using EVCharging.WebApi.Domain;
using EVCharging.WebApi.Infrastructure.Repositories;

namespace EVCharging.WebApi.Services
{
    public class StationService
    {
        private readonly StationRepository _stations;
        private readonly BookingRepository _bookings;

        public StationService(StationRepository stations, BookingRepository bookings)
        {
            _stations = stations;
            _bookings = bookings;
        }

        public Task CreateAsync(Station s) => _stations.CreateAsync(s);
        public Task<List<Station>> GetAllAsync() => _stations.GetAllAsync();
        public Task<Station?> GetByIdAsync(string id) => _stations.FindByIdAsync(id);

        public async Task UpdateAsync(string id, Station updated)
        {
            var existing = await _stations.FindByIdAsync(id)
                ?? throw new KeyNotFoundException($"Station with ID {id} not found.");

            // Merge fields — only replace if new values provided
            var merged = new Station
            {
                Id = existing.Id,
                Name = !string.IsNullOrWhiteSpace(updated.Name) ? updated.Name : existing.Name,
                Type = (!string.IsNullOrWhiteSpace(updated.Type) && updated.Type != "AC")
               ? updated.Type
               : existing.Type,
                Slots = updated.Slots != 0 ? updated.Slots : existing.Slots,
                IsActive = existing.IsActive,
                Location = (updated.Location != null &&
                    (updated.Location.Lat != 0 || updated.Location.Lng != 0))
                   ? updated.Location
                   : existing.Location,
                Availability = (updated.Availability != null && updated.Availability.Any())
                                ? updated.Availability
                                : existing.Availability
            };

            await _stations.UpdateAsync(merged);
        }

        public async Task SetActiveAsync(string id, bool active)
        {
            if (!active)
            {
                // deactivation: ensure no active/upcoming bookings
                var stationBookings = await _bookings.GetByStationAsync(id);
                var blocking = stationBookings.Any(b => b.Status is "Pending" or "Approved");
                if (blocking) throw new InvalidOperationException("Cannot deactivate: active bookings exist.");
            }
            await _stations.UpdateActiveAsync(id, active);
        }

        public Task<List<StationSlotWindow>> GetAvailabilityAsync(string stationId, DateTime fromUtc, DateTime toUtc)
    => _stations.GetAvailabilityAsync(stationId, fromUtc, toUtc);

        public Task<bool> TryReserveSlotAsync(string stationId, DateTime startUtc, DateTime endUtc)
            => _stations.TryReserveSlotAsync(stationId, startUtc, endUtc);

        public Task<bool> ReleaseSlotAsync(string stationId, DateTime startUtc, DateTime endUtc)
            => _stations.ReleaseSlotAsync(stationId, startUtc, endUtc);
    }
}
