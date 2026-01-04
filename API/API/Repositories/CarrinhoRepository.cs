using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class CarrinhoRepository : ICarrinhoRepository
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarrinhoCompras>> GetCarrinhoDoCliente(string userId)
        {
            return await _context.Set<CarrinhoCompras>()
                .Include(c => c.Produto) // Carregar dados do produto (Nome, Preço)
                .Where(c => c.ClienteId == userId)
                .ToListAsync();
        }

        public async Task AdicionarItem(string userId, int produtoId, int quantidade)
        {
            // 1. Buscar o Produto para ver o Stock real
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null) throw new Exception("Produto não existe.");

            var itemExistente = await _context.Set<CarrinhoCompras>()
                .FirstOrDefaultAsync(c => c.ClienteId == userId && c.ProdutoId == produtoId);

            // 2. Calcular quantidade final pretendida
            int qtdFinal = quantidade;
            if (itemExistente != null)
            {
                qtdFinal += itemExistente.Quantidade;
            }

            // 3. VERIFICAÇÃO DE STOCK
            if (qtdFinal > produto.Stock)
            {
                throw new InvalidOperationException($"Stock insuficiente! Só existem {produto.Stock} unidades.");
            }

            // 4. Gravar
            if (itemExistente != null)
            {
                itemExistente.Quantidade = qtdFinal;
            }
            else
            {
                var novoItem = new CarrinhoCompras
                {
                    ClienteId = userId,
                    ProdutoId = produtoId,
                    Quantidade = quantidade
                };
                _context.Add(novoItem);
            }
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarQuantidade(string userId, int produtoId, int quantidade)
        {
            var item = await _context.Set<CarrinhoCompras>()
                .Include(c => c.Produto) // Carregar dados do produto para ver o Stock
                .FirstOrDefaultAsync(c => c.ClienteId == userId && c.ProdutoId == produtoId);

            if (item != null)
            {
                if (quantidade <= 0)
                {
                    _context.Remove(item);
                }
                else
                {
                    // VERIFICAÇÃO DE STOCK AQUI
                    if (item.Produto != null && quantidade > item.Produto.Stock)
                    {
                        throw new InvalidOperationException($"Stock insuficiente! Só existem {item.Produto.Stock} unidades.");
                    }

                    item.Quantidade = quantidade;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoverItem(string userId, int produtoId)
        {
            var item = await _context.Set<CarrinhoCompras>()
                .FirstOrDefaultAsync(c => c.ClienteId == userId && c.ProdutoId == produtoId);

            if (item != null)
            {
                _context.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task LimparCarrinho(string userId)
        {
            var itens = await _context.Set<CarrinhoCompras>()
                .Where(c => c.ClienteId == userId)
                .ToListAsync();

            _context.RemoveRange(itens);
            await _context.SaveChangesAsync();
        }
    }
}