using API.Entities;

namespace API.Repositories
{
    public interface ICarrinhoRepository
    {
        Task<IEnumerable<CarrinhoCompras>> GetCarrinhoDoCliente(string userId);
        Task AdicionarItem(string userId, int produtoId, int quantidade);
        Task AtualizarQuantidade(string userId, int produtoId, int quantidade);
        Task RemoverItem(string userId, int produtoId);
        Task LimparCarrinho(string userId);
    }
}