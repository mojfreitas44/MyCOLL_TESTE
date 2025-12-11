using API.Entities;

namespace API.Repositories
{
    public interface IProdutoRepository
    {
        // Leitura (Público)
        Task<IEnumerable<Produto>> GetAllAsync(string? pesquisa, int? categoriaId);
        Task<Produto?> GetByIdAsync(int id);
        Task<Produto?> GetProdutoDestaqueAsync();
        // Escrita (Fornecedor)
        Task<IEnumerable<Produto>> GetMeusProdutosAsync(string fornecedorId); // Só os meus
        Task<Produto> CriarProdutoAsync(Produto produto);
        Task AtualizarProdutoAsync(Produto produto);
        Task ApagarProdutoAsync(int id);

        // Verifica se o produto pertence a este fornecedor (Segurança)
        Task<bool> SouDonoDoProduto(int produtoId, string fornecedorId);
    }
}