using RCLAPI.DTO;
using System.Net.Http.Json;

namespace RCLAPI.Services
{
    public class ProdutosCliente
    {
        private readonly HttpClient _httpClient;

        public ProdutosCliente(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // --- ESTE É O MÉTODO QUE FALTAVA ---
        public async Task<List<ProdutoDTO>> GetProdutos()
        {
            try
            {
                // Tenta buscar a lista à API. Se der erro ou vier null, devolve lista vazia.
                var resultado = await _httpClient.GetFromJsonAsync<List<ProdutoDTO>>("api/Produtos");
                return resultado ?? new List<ProdutoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar produtos: {ex.Message}");
                return new List<ProdutoDTO>();
            }
        }
    }
}