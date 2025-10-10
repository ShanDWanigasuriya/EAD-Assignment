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

            var station = await _stations.FindByIdAsync(stationId) ?? throw new KeyNotFoundException("Station not found.");
            if (!station.IsActive) throw new InvalidOperationException("Station inactive.");

            // Reserve capacity atomically (only if a window fully covers [start,end] and has slots)
            var reserved = await _stations.TryReserveSlotAsync(stationId, startUtc, endUtc);
            if (!reserved) throw new InvalidOperationException("No capacity in the selected time window.");

            var booking = new Booking
            {
                OwnerNic = ownerNic,
                StationId = stationId,
                ReservationStartUtc = startUtc,
                ReservationEndUtc = endUtc,
                Status = "Pending",
                CreatedAtUtc = nowUtc
            };

            try
            {
                await _bookings.CreateAsync(booking);
                return booking.Id;
            }
            catch
            {
                // Rollback capacity if DB insert fails
                await _stations.ReleaseSlotAsync(stationId, startUtc, endUtc);
                throw;
            }
        }


        public async Task UpdateAsync(string id, DateTime newStartUtc, DateTime newEndUtc, DateTime nowUtc)
        {
            var b = await _bookings.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found.");
            if (!DateRules.IsAtLeast12HoursBefore(b.ReservationStartUtc, nowUtc))
                throw new InvalidOperationException("Updates allowed only >= 12 hours before.");

            // Reserve new window first
            var reserved = await _stations.TryReserveSlotAsync(b.StationId, newStartUtc, newEndUtc);
            if (!reserved) throw new InvalidOperationException("No capacity in the selected new window.");

            try
            {
                // Release old window and save new times
                await _stations.ReleaseSlotAsync(b.StationId, b.ReservationStartUtc, b.ReservationEndUtc);
                b.ReservationStartUtc = newStartUtc;
                b.ReservationEndUtc = newEndUtc;
                b.UpdatedAtUtc = nowUtc;
                await _bookings.UpdateAsync(b);
            }
            catch
            {
                // If anything fails after reserving new, release it to avoid leaks
                await _stations.ReleaseSlotAsync(b.StationId, newStartUtc, newEndUtc);
                throw;
            }
        }

        public async Task CancelAsync(string id, DateTime nowUtc)
        {
            var b = await _bookings.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found.");
            if (!DateRules.IsAtLeast12HoursBefore(b.ReservationStartUtc, nowUtc))
                throw new InvalidOperationException("Cancel allowed only >= 12 hours before.");

            // Release capacity back to the window
            await _stations.ReleaseSlotAsync(b.StationId, b.ReservationStartUtc, b.ReservationEndUtc);

            b.Status = "Cancelled";
            b.UpdatedAtUtc = nowUtc;
            await _bookings.UpdateAsync(b);
        }

        public Task<List<Booking>> GetByOwnerAsync(string nic) => _bookings.GetByOwnerNicAsync(nic);
        public Task<List<Booking>> GetByStationAsync(string stationId) => _bookings.GetByStationAsync(stationId);

        public async Task ApproveAsync(string id)
        {
            var b = await _bookings.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Booking not found.");

            // Only allow approval if status is "Pending"
            if (!string.Equals(b.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Cannot approve booking with status '{b.Status}'. Only Pending bookings can be approved.");

            // Proceed with approval
            b.Status = "Approved";
            b.QrPayload = QrCodeUtil.GeneratePngBase64($"{b.Id}|{b.OwnerNic}");
            b.UpdatedAtUtc = DateTime.UtcNow;

            await _bookings.UpdateAsync(b);
        }

        public async Task CompleteAsync(string id)
        {
            var b = await _bookings.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Booking not found.");

            // Only allow completion if status is "Approved"
            if (!string.Equals(b.Status, "Approved", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Cannot complete booking with status '{b.Status}'. Only Approved bookings can be completed.");

            b.Status = "Completed";
            b.UpdatedAtUtc = DateTime.UtcNow;

            await _bookings.UpdateAsync(b);
        }

    }
}
