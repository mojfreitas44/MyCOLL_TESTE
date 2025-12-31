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

        // No ficheiro FrontEnd/RCLAPI/Services/ProdutosCliente.cs

        public async Task<List<ProdutoDTO>> GetProdutos(string? pesquisa = null, int? categoriaId = null)
        {
            try
            {
                // Constrói a URL com Query String
                var query = "api/Produtos?";

                if (!string.IsNullOrEmpty(pesquisa))
                    query += $"pesquisa={Uri.EscapeDataString(pesquisa)}&";

                if (categoriaId.HasValue)
                    query += $"categoriaId={categoriaId}&";

                var resultado = await _httpClient.GetFromJsonAsync<List<ProdutoDTO>>(query, _options);
                return resultado ?? new List<ProdutoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                return new List<ProdutoDTO>();
            }
        }

        // ... manter o resto (GetProduto, GetProdutoDestaque) igual ...
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
        public async Task<ProdutoDTO?> GetProdutoDestaque()
        {
            try
            {
                var resultado = await _httpClient.GetFromJsonAsync<ProdutoDTO>("api/Produtos/destaque", _options);
                return resultado;
            }
            catch
            {
                return null;
            }
        }
    }
}