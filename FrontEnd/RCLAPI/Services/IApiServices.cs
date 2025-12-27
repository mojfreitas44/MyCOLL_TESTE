using Microsoft.AspNetCore.Components.Forms;
using RCLAPI.DTO;

namespace RCLAPI.Services
{
    public interface IApiServices
    {
        Task<ApiResponse<bool>> RegistarUtilizador(RegisterModel registerModel);
        Task<ApiResponse<bool>> Login(UtilizadorLoginModel login);
        Task<List<Categoria>> GetCategorias();
        Task<List<ProdutoDTO>> GetProdutosEspecificos(string produtoTipo, int? IdCategoria);
        Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint);
        Task<ProdutoListDTO?> GetProdutoDestaqueAsync();
        Task<ProdutoDetaislDTO?> GetProdutoDetalhe(int id);

        // Encomendas
        Task<List<EncomendaDTO>> ListarMinhasEncomendas();
        Task<EncomendaDTO?> GetEncomenda(int id);
        Task<(bool ok, string? err, EncomendaDTO? encomenda)> Checkout(CheckoutDTO dto);
        Task<bool> ConfirmarRecebido(int encomendaId);
        Task<(bool ok, string? error)> PagarEncomenda(int encomendaId, PagamentoDTO dto);
        Task<List<EncomendaDTO>?> GetMinhasEncomendas();

        // Carrinho
        Task<ApiResponse<bool>> AdicionaItemNoCarrinho(ItemCarrinhoCompra carrinhoCompra);
        Task<List<ItemCarrinhoCompra>> GetCarrinhoItems();
        Task<bool> AtualizarQuantidadeProduto(int produtoId, int quantidade);
        Task<bool> RemoveItemFromCarrinho(int produtoId);
        Task<bool> ClearCarrinho();

        // Favoritos
        Task<List<ProdutoFavorito>> GetFavoritos();
        Task<List<int>> GetFavoritosIds();
        Task<(bool Data, string? ErrorMessage)> AdicionarFavorito(int produtoId);
        Task<(bool Data, string? ErrorMessage)> RemoverFavorito(int produtoId);
        Task<(bool Data, string? ErrorMessage)> ToggleFavorito(int produtoId);
        Task<(bool Data, string? ErrorMessage)> SyncFavoritos(IEnumerable<int> ids);
        Task<(bool Data, string? ErrorMessage)> ClearFavoritos();
        Task<bool> ValidaSessao();

        // Fornecedor
        Task<List<ProdutoFornecedorDTO>> Forn_ListarMeusProdutos();
        Task<ProdutoFornecedorDTO?> Forn_ObterMeuProduto(int id);
        Task<(bool Ok, string? Error)> Forn_CriarProduto(ProdutoFornecedorDTO dto, IBrowserFile? image);
        Task<(bool Ok, string? Error)> Forn_AtualizarProduto(int id, ProdutoFornecedorDTO dto, IBrowserFile? image);
        Task<(bool Ok, string? Error)> Forn_DefinirDisponivel(int id, bool disponivel);
        Task<List<ListaVendasItemDTO>?> GetFornecedorListaVendas(DateTime? de = null, DateTime? ate = null, string? status = null);
        Task<(bool Ok, string? Error)> Forn_ApagarProduto(int id);

        // Outros
        Task<List<CategoriaDTO>> GetCategoria();
        Task<List<ModoEntregaDTO>> GetModosDeEntrega();

        // Reservas
        Task<(bool Ok, string? Error)> CriarReserva(ReservaCreateDTO dto);
        Task<List<ReservaDTO>> ListarMinhasReservas();
        Task<(bool Ok, string? Error)> CancelarReserva(int id);
    }
}