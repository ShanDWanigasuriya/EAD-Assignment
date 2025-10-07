using EVCharging.WebApi.Domain;
using EVCharging.WebApi.Infrastructure.Repositories;
using EVCharging.WebApi.Utils;

namespace EVCharging.WebApi.Services
{
    public class BookingService
    {
        private readonly BookingRepository _bookings;
        private readonly OwnerRepository _owners;
        private readonly StationRepository _stations;

        public BookingService(BookingRepository bookings, OwnerRepository owners, StationRepository stations)
        {
            _bookings = bookings;
            _owners = owners;
            _stations = stations;
        }

        public async Task<string> CreateAsync(string ownerNic, string stationId, DateTime startUtc, DateTime endUtc, DateTime nowUtc)
        {
            if (!DateRules.IsWithin7Days(startUtc, nowUtc))
                throw new InvalidOperationException("Reservation must be within 7 days.");

            var owner = await _owners.FindByNicAsync(ownerNic) ?? throw new KeyNotFoundException("Owner not found.");
            if (!owner.IsActive) throw new InvalidOperationException("Owner is inactive.");

            var station = await _stations.GetByIdAsync(stationId) ?? throw new KeyNotFoundException("Station not found.");
            if (!station.IsActive) throw new InvalidOperationException("Station inactive.");

            // TODO: Optional slot capacity checks against station.Availability windows

            var booking = new Booking
            {
                OwnerNic = ownerNic,
                StationId = stationId,
                ReservationStartUtc = startUtc,
                ReservationEndUtc = endUtc,
                Status = "Pending"
            };

            await _bookings.CreateAsync(booking);
            return booking.Id;
        }

        public async Task UpdateAsync(string id, DateTime newStartUtc, DateTime newEndUtc, DateTime nowUtc)
        {
            var b = await _bookings.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found.");
            if (!DateRules.IsAtLeast12HoursBefore(b.ReservationStartUtc, nowUtc))
                throw new InvalidOperationException("Updates allowed only >= 12 hours before.");

            b.ReservationStartUtc = newStartUtc;
            b.ReservationEndUtc = newEndUtc;
            b.UpdatedAtUtc = nowUtc;
            await _bookings.UpdateAsync(b);
        }

        public async Task CancelAsync(string id, DateTime nowUtc)
        {
            var b = await _bookings.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found.");
            if (!DateRules.IsAtLeast12HoursBefore(b.ReservationStartUtc, nowUtc))
                throw new InvalidOperationException("Cancel allowed only >= 12 hours before.");
            b.Status = "Cancelled";
            b.UpdatedAtUtc = nowUtc;
            await _bookings.UpdateAsync(b);
        }

        public Task<List<Booking>> GetByOwnerAsync(string nic) => _bookings.GetByOwnerNicAsync(nic);
        public Task<List<Booking>> GetByStationAsync(string stationId) => _bookings.GetByStationAsync(stationId);

        public async Task ApproveAsync(string id)
        {
            var b = await _bookings.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found.");
            b.Status = "Approved";
            b.QrPayload = QrCodeUtil.GeneratePngBase64($"{b.Id}|{b.OwnerNic}");
            await _bookings.UpdateAsync(b);
        }

        public async Task CompleteAsync(string id)
        {
            var b = await _bookings.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found.");
            b.Status = "Completed";
            b.UpdatedAtUtc = DateTime.UtcNow;
            await _bookings.UpdateAsync(b);
        }
    }
}
