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
        public Task<Station?> GetByIdAsync(string id) => _stations.GetByIdAsync(id);

        public Task UpdateAsync(Station s) => _stations.ReplaceAsync(s);

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
    }
}
