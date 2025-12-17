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
            // --- 1. BLOQUEAR MÉTODOS ESTRANHOS ---
            var metodosAceites = new[] { "Visa", "Mastercard" };

            if (!metodosAceites.Contains(dto.MetodoPagamento))
            {
                throw new Exception($"Método de pagamento inválido. Apenas aceitamos: {string.Join(", ", metodosAceites)}.");
            }

            // --- 2. VALIDAR MODO DE ENTREGA ---
            var modoEntrega = await _context.ModosEntrega.FindAsync(dto.ModoEntregaId);
            if (modoEntrega == null)
            {
                throw new Exception("Modo de entrega inválido.");
            }

            // --- 3. VALIDAR DADOS DO CARTÃO ---
            if (string.IsNullOrEmpty(dto.NumeroCartao) || dto.NumeroCartao.Length < 16)
            {
                throw new Exception("Pagamento Recusado: Cartão inválido (Simulação: use 16 dígitos).");
            }
            if (string.IsNullOrEmpty(dto.CVV) || dto.CVV.Length < 3)
            {
                throw new Exception("Pagamento Recusado: CVV inválido.");
            }

            // --- 4. BUSCAR O CARRINHO ---
            var itensCarrinho = await _context.Set<CarrinhoCompras>()
                .Include(c => c.Produto)
                .Where(c => c.ClienteId == userId)
                .ToListAsync();

            if (!itensCarrinho.Any()) throw new Exception("O carrinho está vazio.");

            // --- 5. CRIAR A ENCOMENDA (CABEÇALHO) ---
            var novaEncomenda = new Encomenda
            {
                ClienteId = userId,
                Data = DateTime.UtcNow,
                Estado = "Pendente",
                MoradaEnvio = dto.MoradaEnvio,
                MetodoPagamento = dto.MetodoPagamento,
                MetodoEntrega = modoEntrega.Nome,
                ValorTotal = modoEntrega.Preco
            };

            // --- 6. PROCESSAR ITENS E STOCK ---
            foreach (var item in itensCarrinho)
            {
                if (item.Produto == null) continue;

                if (item.Produto.Stock < item.Quantidade)
                {
                    throw new Exception($"Stock insuficiente para o produto '{item.Produto.Nome}'. Restam apenas {item.Produto.Stock}.");
                }

                item.Produto.Stock -= item.Quantidade;

                var encomendaItem = new EncomendaItem
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.Produto.PrecoVenda
                };

                novaEncomenda.Itens.Add(encomendaItem);
                novaEncomenda.ValorTotal += (encomendaItem.PrecoUnitario * encomendaItem.Quantidade);
            }

            // --- 7. GRAVAR TUDO E LIMPAR CARRINHO ---
            _context.Set<Encomenda>().Add(novaEncomenda);
            _context.Set<CarrinhoCompras>().RemoveRange(itensCarrinho);

            await _context.SaveChangesAsync();

            return novaEncomenda;
        }

        // --- MÉTODOS DE CLIENTE ---
        public async Task<IEnumerable<Encomenda>> GetEncomendasDoCliente(string userId)
        {
            return await _context.Set<Encomenda>()
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto) // Importante incluir o Produto para ver o nome
                .Where(e => e.ClienteId == userId)
                .OrderByDescending(e => e.Data)
                .ToListAsync();
        }

        public async Task<Encomenda?> GetDetalhesEncomenda(string userId, int encomendaId)
        {
            return await _context.Set<Encomenda>()
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(e => e.Id == encomendaId && e.ClienteId == userId);
        }

        // --- MÉTODOS DE ADMIN (NOVOS) ---
        public async Task<IEnumerable<Encomenda>> GetAllEncomendas()
        {
            return await _context.Set<Encomenda>()
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
                .OrderByDescending(e => e.Data)
                .ToListAsync();
        }

        public async Task<Encomenda?> GetEncomendaPorId(int id)
        {
            return await _context.Set<Encomenda>()
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        // -------------------------------

        public async Task<IEnumerable<VendaFornecedorDTO>> GetVendasDoFornecedor(string fornecedorId)
        {
            return await _context.Set<EncomendaItem>()
                .Include(i => i.Encomenda)
                .Include(i => i.Produto)
                .Where(i => i.Produto.FornecedorId == fornecedorId)
                .OrderByDescending(i => i.Encomenda.Data)
                .Select(i => new VendaFornecedorDTO
                {
                    EncomendaId = i.EncomendaId,
                    DataVenda = i.Encomenda.Data,
                    NomeProduto = i.Produto.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    TotalGanho = i.Quantidade * i.PrecoUnitario,
                    EstadoEncomenda = i.Encomenda.Estado
                })
                .ToListAsync();
        }
    }
}