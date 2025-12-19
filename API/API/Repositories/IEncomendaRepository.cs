using API.DTO;
using API.Entities;

namespace API.Repositories
{
    public interface IEncomendaRepository
    {
        // Cria a encomenda a partir do carrinho atual
        Task<Encomenda> CriarEncomenda(string userId, CheckoutDto dto);

        // --- MÉTODOS PARA CLIENTES (FILTRADOS POR USER) ---
        // Mostra o histórico de encomendas do cliente
        Task<IEnumerable<Encomenda>> GetEncomendasDoCliente(string userId);

        // Mostra detalhes de uma encomenda específica (se pertencer ao cliente)
        Task<Encomenda?> GetDetalhesEncomenda(string userId, int encomendaId);

        // Admin vê TODAS as encomendas da loja
        Task<IEnumerable<Encomenda>> GetAllEncomendas();

        // Admin vê detalhes de QUALQUER encomenda pelo ID
        Task<Encomenda?> GetEncomendaPorId(int id);

        // Permite alterar o estado (ex: "Entregue", "Enviado", "Pago")
        Task AtualizarEstado(int encomendaId, string novoEstado);
        // Para a área de fornecedor ver as suas vendas
        Task<IEnumerable<VendaFornecedorDTO>> GetVendasDoFornecedor(string fornecedorId);
    }
}