using API.DTO;

namespace API.Repositories
{
    public interface IFavoritoRepository
    {
        Task<IEnumerable<ProdutoDTO>> GetFavoritosDoCliente(string userId);
        Task AdicionarFavorito(string userId, int produtoId);
        Task RemoverFavorito(string userId, int produtoId);
    }
}