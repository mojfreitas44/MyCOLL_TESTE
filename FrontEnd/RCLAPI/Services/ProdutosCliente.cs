using System.Net.Http.Json;
using RCLAPI.DTO;

namespace RCLAPI.Services
{
    public class ProdutosClient
    {
        private readonly HttpClient _httpClient;

        public ProdutosClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProdutoDTO>> GetProdutosAsync()
        {
            try
            {
                // Atenção: O endpoint na API é "api/Produtos"
                var lista = await _httpClient.GetFromJsonAsync<List<ProdutoDTO>>("api/Produtos");
                return lista ?? new List<ProdutoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO API: {ex.Message}");
                return new List<ProdutoDTO>();
            }
        }
    }
}