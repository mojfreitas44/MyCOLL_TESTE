using RCLAPI.DTO;
using System.Net.Http.Json;
using System.Text.Json;

namespace RCLAPI.Services
{
    public class ProdutosCliente
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public ProdutosCliente(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Isto permite ler "paraVenda" ou "ParaVenda" sem problemas
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<ProdutoDTO>> GetProdutos()
        {
            try
            {
                var resultado = await _httpClient.GetFromJsonAsync<List<ProdutoDTO>>("api/Produtos", _options);
                return resultado ?? new List<ProdutoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                return new List<ProdutoDTO>();
            }
        }
        // No ficheiro: FrontEnd/RCLAPI/Services/ProdutosCliente.cs

        public async Task<ProdutoDTO?> GetProduto(int id)
        {
            try
            {
                // Chama a API: GET api/Produtos/{id}
                var resultado = await _httpClient.GetFromJsonAsync<ProdutoDTO>($"api/Produtos/{id}", _options);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar produto {id}: {ex.Message}");
                return null;
            }
        }
    }
}