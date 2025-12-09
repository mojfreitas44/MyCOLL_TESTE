using API.Data;
using API.DTO;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class EncomendaRepository : IEncomendaRepository
    {
        private readonly ApplicationDbContext _context;

        public EncomendaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Encomenda> CriarEncomenda(string userId, CheckoutDto dto)
        {
            // 1. Buscar o Carrinho do Cliente (com os preços dos produtos)
            var itensCarrinho = await _context.Set<CarrinhoCompras>()
                .Include(c => c.Produto)
                .Where(c => c.ClienteId == userId)
                .ToListAsync();

            if (!itensCarrinho.Any())
            {
                throw new Exception("O carrinho está vazio.");
            }

            // 2. Criar a Encomenda (Cabeçalho)
            var novaEncomenda = new Encomenda
            {
                ClienteId = userId,
                Data = DateTime.UtcNow,
                Estado = "Pendente",
                MoradaEnvio = dto.MoradaEnvio,
                MetodoPagamento = dto.MetodoPagamento,
                MetodoEntrega = dto.MetodoEntrega,
                ValorTotal = 0 // Vamos calcular já a seguir
            };

            // 3. Converter Itens do Carrinho em Itens da Encomenda
            foreach (var item in itensCarrinho)
            {
                if (item.Produto == null) continue;

                var encomendaItem = new EncomendaItem
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.Produto.PrecoVenda // Fixamos o preço no momento da compra
                };

                novaEncomenda.Itens.Add(encomendaItem);
                novaEncomenda.ValorTotal += (encomendaItem.PrecoUnitario * encomendaItem.Quantidade);
            }

            // 4. Guardar Encomenda
            _context.Set<Encomenda>().Add(novaEncomenda);

            // 5. Limpar o Carrinho (Já não precisamos dele)
            _context.Set<CarrinhoCompras>().RemoveRange(itensCarrinho);

            // 6. Gravar tudo na BD de uma vez (Transação)
            await _context.SaveChangesAsync();

            return novaEncomenda;
        }

        public async Task<IEnumerable<Encomenda>> GetEncomendasDoCliente(string userId)
        {
            return await _context.Set<Encomenda>()
                .Include(e => e.Itens) // Inclui os itens para contarmos quantos são
                .Where(e => e.ClienteId == userId)
                .OrderByDescending(e => e.Data)
                .ToListAsync();
        }

        public async Task<Encomenda?> GetDetalhesEncomenda(string userId, int encomendaId)
        {
            return await _context.Set<Encomenda>()
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto) // Para ver o nome do produto no detalhe
                .FirstOrDefaultAsync(e => e.Id == encomendaId && e.ClienteId == userId);
        }
    }
}