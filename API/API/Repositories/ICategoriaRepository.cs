using API.Entities;

namespace API.Repositories
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetCategorias();
        Task<Categoria?> GetCategoria(int id);
    }
}