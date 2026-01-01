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
            // 1. VALIDAR PAGAMENTO (SÓ VISA/MASTERCARD)
            var metodosAceites = new[] { "Visa", "Mastercard" };
            if (!metodosAceites.Contains(dto.MetodoPagamento))
            {
                throw new Exception($"Método inválido. Aceitamos apenas: {string.Join(", ", metodosAceites)}.");
            }

            // 2. VALIDAR CARTÃO
            if (string.IsNullOrEmpty(dto.NumeroCartao) || dto.NumeroCartao.Length < 15)
                throw new Exception("Cartão inválido.");

            // 3. VALIDAR MODO DE ENTREGA
            var modoEntrega = await _context.ModosEntrega.FindAsync(dto.ModoEntregaId);
            if (modoEntrega == null) throw new Exception("Modo de entrega inválido.");

            // 4. VALIDAR SE O CARRINHO TEM ITENS (Vem do Frontend)
            if (dto.Itens == null || !dto.Itens.Any())
            {
                throw new Exception("O carrinho está vazio.");
            }

            // 5. CRIAR ENCOMENDA
            var encomenda = new Encomenda
            {
                ClienteId = userId,
                Data = DateTime.Now,
                Estado = "Pendente",
                MoradaEnvio = dto.MoradaEnvio,
                MetodoPagamento = dto.MetodoPagamento,
                MetodoEntrega = modoEntrega.Nome, // Guardamos o nome porque a tua tabela não tem o ID
                ValorTotal = 0,
                Itens = new List<EncomendaItem>()
            };

            _context.Encomendas.Add(encomenda); // Prepara para gerar ID

            decimal totalItens = 0;

            // 6. PROCESSAR PRODUTOS
            foreach (var itemDto in dto.Itens)
            {
                // Buscar produto à BD para garantir preço e stock reais
                var produto = await _context.Produtos.FindAsync(itemDto.ProdutoId);

                if (produto == null)
                    throw new Exception($"Produto {itemDto.ProdutoId} não existe.");

                if (produto.Stock < itemDto.Quantidade)
                    throw new Exception($"Stock insuficiente para '{produto.Nome}'.");

                // Criar linha da encomenda
                var linha = new EncomendaItem
                {
                    Encomenda = encomenda,
                    ProdutoId = produto.Id,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.PrecoVenda // CORRIGIDO: Usar PrecoVenda
                };

                // Abater ao Stock
                produto.Stock -= itemDto.Quantidade;

                // Somar
                totalItens += (linha.PrecoUnitario * linha.Quantidade);

                encomenda.Itens.Add(linha);
            }

            // 7. CALCULAR TOTAL (Produtos + Portes)
            encomenda.ValorTotal = totalItens + modoEntrega.Preco;

            // 8. GRAVAR
            await _context.SaveChangesAsync();

            return encomenda;
        }

        // --- OUTROS MÉTODOS (MANTÊM-SE IGUAIS, SÓ VERIFICAR O INCLUDE) ---

        public async Task<IEnumerable<Encomenda>> GetEncomendasDoCliente(string userId)
        {
            return await _context.Encomendas
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
                .Where(e => e.ClienteId == userId)
                .OrderByDescending(e => e.Data)
                .ToListAsync();
        }

        public async Task<IEnumerable<Encomenda>> GetAllEncomendas()
        {
            return await _context.Encomendas
                .Include(e => e.Cliente)
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
                .OrderByDescending(e => e.Data)
                .ToListAsync();
        }

        public async Task<Encomenda?> GetDetalhesEncomenda(string userId, int encomendaId)
        {
            return await _context.Encomendas
                .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(e => e.Id == encomendaId && e.ClienteId == userId);
        }

        public async Task<Encomenda?> GetEncomendaPorId(int id)
        {
            return await _context.Encomendas
               .Include(e => e.Cliente)
               .Include(e => e.Itens)
               .ThenInclude(i => i.Produto)
               .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AtualizarEstado(int encomendaId, string novoEstado)
        {
            var encomenda = await _context.Encomendas.FindAsync(encomendaId);
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