using API.DTO;
using API.Entities;

namespace API.Repositories
{
    public interface IEncomendaRepository
    {
        // Cria a encomenda a partir do carrinho atual
        Task<Encomenda> CriarEncomenda(string userId, CheckoutDto dto);

        // Mostra o histórico de encomendas do cliente
        Task<IEnumerable<Encomenda>> GetEncomendasDoCliente(string userId);

        // Mostra detalhes de uma encomenda específica
        Task<Encomenda?> GetDetalhesEncomenda(string userId, int encomendaId);
    }
}