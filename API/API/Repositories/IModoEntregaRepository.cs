using API.Entities;

namespace API.Repositories
{
    public interface IModoEntregaRepository
    {
        Task<IEnumerable<ModoEntrega>> GetAllAsync();
        Task<ModoEntrega?> GetByIdAsync(int id);
    }
}