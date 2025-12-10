using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Produto>> GetAllAsync(string? pesquisa, int? categoriaId)
        {
            var query = _context.Produtos
                .AsNoTracking()
                .Where(p => p.ParaVenda == true)
                .Where(p => p.Estado == "Ativo"); // <--- MUDANÇA AQUI (Era "Aprovado")

            if (!string.IsNullOrEmpty(pesquisa))
            {
                query = query.Where(p => p.Nome.Contains(pesquisa) || p.Descricao.Contains(pesquisa));
            }

            if (categoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaId == categoriaId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Produto?> GetByIdAsync(int id)
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Produto>> GetMeusProdutosAsync(string fornecedorId)
        {
            return await _context.Produtos
                .Where(p => p.FornecedorId == fornecedorId)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<Produto> CriarProdutoAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task AtualizarProdutoAsync(Produto produto)
        {
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task ApagarProdutoAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SouDonoDoProduto(int produtoId, string fornecedorId)
        {
            return await _context.Produtos
                .AnyAsync(p => p.Id == produtoId && p.FornecedorId == fornecedorId);
        }
    }
}