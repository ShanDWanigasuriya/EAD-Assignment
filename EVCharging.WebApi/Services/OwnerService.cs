using EVCharging.WebApi.Domain;
using EVCharging.WebApi.Infrastructure.Repositories;

namespace EVCharging.WebApi.Services
{
    public class OwnerService
    {
        private readonly OwnerRepository _owners;
        public OwnerService(OwnerRepository owners) => _owners = owners;

        // Register new owner (NIC must be unique)
        public async Task RegisterAsync(Owner o)
        {
            var existing = await _owners.FindByNicAsync(o.NIC);
            if (existing != null)
                throw new InvalidOperationException($"Owner with NIC {o.NIC} already exists.");

            o.IsActive = true;
            await _owners.CreateAsync(o);
        }

        // Get owner by NIC
        public Task<Owner?> GetByNicAsync(string nic) => _owners.FindByNicAsync(nic);

        // Update owner by NIC (partial update)
        public async Task UpdateAsync(string nic, Owner updated)
        {
            var existing = await _owners.FindByNicAsync(nic)
                ?? throw new KeyNotFoundException($"Owner with NIC {nic} not found.");

            // Merge updated fields with existing
            var merged = new Owner
            {
                Id = existing.Id,
                NIC = existing.NIC,
                FullName = !string.IsNullOrWhiteSpace(updated.FullName) ? updated.FullName : existing.FullName,
                Email = !string.IsNullOrWhiteSpace(updated.Email) ? updated.Email : existing.Email,
                Phone = !string.IsNullOrWhiteSpace(updated.Phone) ? updated.Phone : existing.Phone,
                IsActive = existing.IsActive,
                CreatedAt = existing.CreatedAt
            };

            await _owners.UpdateByNicAsync(nic, merged);
        }

        // Deactivate / Reactivate
        public Task DeactivateAsync(string nic) => _owners.UpdateActiveByNicAsync(nic, false);
        public Task ReactivateAsync(string nic) => _owners.UpdateActiveByNicAsync(nic, true);

        // Get all owners
        public Task<List<Owner>> GetAllAsync() => _owners.GetAllAsync();

        // Delete owner
        public Task DeleteAsync(string nic) => _owners.DeleteByNicAsync(nic);
    }
}
