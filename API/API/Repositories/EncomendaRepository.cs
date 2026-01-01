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
            // 1. Validar Pagamento
            var metodosAceites = new[] { "Visa", "Mastercard" };
            if (!metodosAceites.Contains(dto.MetodoPagamento))
            {
                throw new Exception($"Método inválido. Aceitamos: {string.Join(", ", metodosAceites)}.");
            }

            if (string.IsNullOrEmpty(dto.NumeroCartao) || dto.NumeroCartao.Length < 15)
                throw new Exception("Cartão inválido.");

            // 2. Validar Entrega
            var modoEntrega = await _context.ModosEntrega.FindAsync(dto.ModoEntregaId);
            if (modoEntrega == null) throw new Exception("Modo de entrega inválido.");

            // 3. Validar Itens
            if (dto.Itens == null || !dto.Itens.Any())
                throw new Exception("Carrinho vazio.");

            // 4. Criar Cabeçalho
            var encomenda = new Encomenda
            {
                ClienteId = userId,
                Data = DateTime.Now,
                Estado = "Pendente",
                MoradaEnvio = dto.MoradaEnvio,
                MetodoPagamento = dto.MetodoPagamento,
                MetodoEntrega = modoEntrega.Nome,
                ValorTotal = 0,
                Itens = new List<EncomendaItem>()
            };

            _context.Set<Encomenda>().Add(encomenda); // Usar Set<T> é mais seguro

            decimal totalItens = 0;

            // 5. Processar Itens
            foreach (var itemDto in dto.Itens)
            {
                var produto = await _context.Produtos.FindAsync(itemDto.ProdutoId);

                if (produto == null)
                    throw new Exception($"Produto {itemDto.ProdutoId} não existe.");

                if (produto.Stock < itemDto.Quantidade)
                    throw new Exception($"Stock insuficiente para '{produto.Nome}'.");

                var linha = new EncomendaItem
                {
                    Encomenda = encomenda,
                    ProdutoId = produto.Id,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.PrecoVenda
                };

                produto.Stock -= itemDto.Quantidade;
                totalItens += (linha.PrecoUnitario * linha.Quantidade);
                encomenda.Itens.Add(linha);
            }

            encomenda.ValorTotal = totalItens + modoEntrega.Preco;
            await _context.SaveChangesAsync();

            return encomenda;
        }

        // --- MÉTODOS DE LEITURA ---

        public async Task<IEnumerable<Encomenda>> GetEncomendasDoCliente(string userId)
        {
            return await _context.Set<Encomenda>()
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
                .Where(e => e.ClienteId == userId)
                .OrderByDescending(e => e.Data)
                .ToListAsync();
        }

        public async Task<IEnumerable<Encomenda>> GetAllEncomendas()
        {
            return await _context.Set<Encomenda>()
                .Include(e => e.Cliente)
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
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

        public async Task<Encomenda?> GetEncomendaPorId(int id)
        {
            return await _context.Set<Encomenda>()
               .Include(e => e.Cliente)
               .Include(e => e.Itens)
               .ThenInclude(i => i.Produto)
               .FirstOrDefaultAsync(e => e.Id == id);
        }

        // --- MÉTODO DE ATUALIZAÇÃO CORRIGIDO ---
        public async Task AtualizarEstado(int encomendaId, string novoEstado)
        {
            // Usamos Set<Encomenda>() para garantir que funciona
            var encomenda = await _context.Set<Encomenda>().FindAsync(encomendaId);

            if (encomenda != null)
            {
                encomenda.Estado = novoEstado;
                await _context.SaveChangesAsync();
            }
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