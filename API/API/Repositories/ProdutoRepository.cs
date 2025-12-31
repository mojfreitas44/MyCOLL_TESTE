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
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                // REMOVIDO: .Where(p => p.ParaVenda == true) <--- Isto impedia a coleção de funcionar
                .Where(p => p.Estado == "Ativo"); // Mantém isto, é a regra de negócio correta

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

        // --- NOVO MÉTODO IMPLEMENTADO ---
        public async Task<Produto?> GetProdutoDestaqueAsync()
        {
            // 1. Contar quantos produtos elegíveis existem
            var count = await _context.Produtos.CountAsync(p => p.Estado == "Ativo" && p.ParaVenda == true);

            if (count == 0) return null;

            // 2. Sortear um índice aleatório
            var random = new Random();
            var skip = random.Next(0, count);

            // 3. Saltar e pegar 1
            return await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .Where(p => p.Estado == "Ativo" && p.ParaVenda == true)
                .Skip(skip)
                .FirstOrDefaultAsync();
        }
        // ---------------------------------

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
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor) 
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
        public async Task<IEnumerable<Produto>> GetProdutosPendentesAsync()
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor) 
                .Where(p => p.Estado == "Pendente")
                .ToListAsync();
        }
    }
}