using API.Data;
using API.DTO;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class FavoritoRepository : IFavoritoRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoritoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProdutoDTO>> GetFavoritosDoCliente(string userId)
        {
            // Vai buscar os favoritos e transforma logo em ProdutoDTO para a App ler
            return await _context.Favoritos
                .AsNoTracking()
                .Where(f => f.ClienteId == userId)
                .Include(f => f.Produto)
                .ThenInclude(p => p!.Categoria)
                .Select(f => new ProdutoDTO
                {
                    Id = f.Produto!.Id,
                    Nome = f.Produto.Nome,
                    Descricao = f.Produto.Descricao,
                    PrecoVenda = f.Produto.PrecoVenda,
                    Imagem = f.Produto.Imagem,
                    CategoriaNome = f.Produto.Categoria != null ? f.Produto.Categoria.Nome : "",
                    Disponibilidade = f.Produto.Estado // ou calcular "Em Stock"
                })
                .ToListAsync();
        }

        public async Task AdicionarFavorito(string userId, int produtoId)
        {
            // Verifica se já existe para não dar erro
            var existe = await _context.Favoritos
                .AnyAsync(f => f.ClienteId == userId && f.ProdutoId == produtoId);

            if (!existe)
            {
                _context.Favoritos.Add(new Favorito { ClienteId = userId, ProdutoId = produtoId });
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoverFavorito(string userId, int produtoId)
        {
            var favorito = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.ClienteId == userId && f.ProdutoId == produtoId);

            if (favorito != null)
            {
                _context.Favoritos.Remove(favorito);
                await _context.SaveChangesAsync();
            }
        }
    }
}