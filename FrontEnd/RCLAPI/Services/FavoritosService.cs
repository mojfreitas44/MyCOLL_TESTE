using RCLAPI.DTO;
using System.Net.Http.Json;

namespace RCLAPI.Services
{
    public class FavoritosService
    {
        private readonly HttpClient _httpClient;

        public FavoritosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProdutoDTO>> ObterFavoritos()
        {
            try
            {
                // Devolve a lista ou lista vazia se der erro
                return await _httpClient.GetFromJsonAsync<List<ProdutoDTO>>("api/Favoritos") ?? new List<ProdutoDTO>();
            }
            catch
            {
                return new List<ProdutoDTO>();
            }
        }

        public async Task<bool> AdicionarFavorito(int produtoId)
        {
            try
            {
                var result = await _httpClient.PostAsync($"api/Favoritos/{produtoId}", null);
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoverFavorito(int produtoId)
        {
            try
            {
                var result = await _httpClient.DeleteAsync($"api/Favoritos/{produtoId}");
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}