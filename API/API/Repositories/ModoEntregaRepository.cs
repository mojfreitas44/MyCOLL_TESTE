using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ModoEntregaRepository : IModoEntregaRepository
    {
        private readonly ApplicationDbContext _context;

        public ModoEntregaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModoEntrega>> GetAllAsync()
        {
            return await _context.ModosEntrega
                .AsNoTracking()
                .OrderBy(m => m.Preco) // Ordena do mais barato para o mais caro
                .ToListAsync();
        }

        public async Task<ModoEntrega?> GetByIdAsync(int id)
        {
            return await _context.ModosEntrega.FindAsync(id);
        }
    }
}