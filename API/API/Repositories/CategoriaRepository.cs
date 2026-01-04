using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetCategorias()
        {
            // Inclui as SubCategorias para carregar a hierarquia se necessário
            return await _context.Categorias
                .Include(c => c.SubCategorias)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<Categoria?> GetCategoria(int id)
        {
            return await _context.Categorias
                .Include(c => c.SubCategorias)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}