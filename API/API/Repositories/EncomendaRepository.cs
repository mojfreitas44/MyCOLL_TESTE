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
            // --- 1. BLOQUEAR MÉTODOS ESTRANHOS (A Alteração) ---
            // Define a lista branca. Se vier "Batatas", rebenta aqui.
            var metodosAceites = new[] { "Visa", "Mastercard" };

            if (!metodosAceites.Contains(dto.MetodoPagamento))
            {
                throw new Exception($"Método de pagamento inválido. Apenas aceitamos: {string.Join(", ", metodosAceites)}.");
            }

            // --- 2. VALIDAR MODO DE ENTREGA ---
            // Verifica se o ID do envio existe na BD
            var modoEntrega = await _context.ModosEntrega.FindAsync(dto.ModoEntregaId);
            if (modoEntrega == null)
            {
                throw new Exception("Modo de entrega inválido.");
            }

            // --- 3. VALIDAR DADOS DO CARTÃO ---
            // Como já obrigámos a ser Visa ou Mastercard no passo 1, validamos SEMPRE os números.
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

                // Guardamos o Nome e Preço atuais para histórico
                MetodoEntrega = modoEntrega.Nome,
                ValorTotal = modoEntrega.Preco
            };

            // --- 6. PROCESSAR ITENS E STOCK ---
            foreach (var item in itensCarrinho)
            {
                if (item.Produto == null) continue;

                // Validação de Stock
                if (item.Produto.Stock < item.Quantidade)
                {
                    throw new Exception($"Stock insuficiente para o produto '{item.Produto.Nome}'. Restam apenas {item.Produto.Stock}.");
                }

                // Descontar do Stock
                item.Produto.Stock -= item.Quantidade;

                // Criar linha da encomenda
                var encomendaItem = new EncomendaItem
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.Produto.PrecoVenda
                };

                novaEncomenda.Itens.Add(encomendaItem);

                // Somar ao total (que já inclui o envio)
                novaEncomenda.ValorTotal += (encomendaItem.PrecoUnitario * encomendaItem.Quantidade);
            }

            // --- 7. GRAVAR TUDO E LIMPAR CARRINHO ---
            _context.Set<Encomenda>().Add(novaEncomenda);
            _context.Set<CarrinhoCompras>().RemoveRange(itensCarrinho);

            await _context.SaveChangesAsync();

            return novaEncomenda;
        }

        public async Task<IEnumerable<Encomenda>> GetEncomendasDoCliente(string userId)
        {
            return await _context.Set<Encomenda>()
                .Include(e => e.Itens)
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