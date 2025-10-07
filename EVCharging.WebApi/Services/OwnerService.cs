using EVCharging.WebApi.Domain;
using EVCharging.WebApi.Infrastructure.Repositories;

namespace EVCharging.WebApi.Services
{
    public class OwnerService
    {
        private readonly OwnerRepository _owners;
        public OwnerService(OwnerRepository owners) => _owners = owners;

        public async Task RegisterAsync(Owner o)
        {
            var existing = await _owners.FindByNicAsync(o.NIC);
            if (existing != null) throw new InvalidOperationException("Owner with NIC exists.");
            o.IsActive = true;
            await _owners.CreateAsync(o);
        }

        public Task<Owner?> GetByNicAsync(string nic) => _owners.FindByNicAsync(nic);

        public async Task UpdateAsync(string nic, Owner updated)
        {
            var existing = await _owners.FindByNicAsync(nic) ?? throw new KeyNotFoundException("Owner not found.");
            updated.Id = existing.Id;
            await _owners.ReplaceAsync(existing.Id, updated);
        }

        public Task DeactivateAsync(string nic) => _owners.UpdateActiveByNicAsync(nic, false);
        public Task ReactivateAsync(string nic) => _owners.UpdateActiveByNicAsync(nic, true);
        public Task<List<Owner>> GetAllAsync() => _owners.GetAllAsync();
    }
}
