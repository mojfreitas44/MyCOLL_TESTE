using RCLAPI.DTO;
using System.Net.Http.Json;

namespace RCLAPI.Services
{
    public class FornecedorService
    {
        private readonly HttpClient _httpClient;

        public FornecedorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. Listar Meus Produtos
        public async Task<List<ProdutoDTO>> GetMeusProdutos()
        {
            try
            {
                // O token JWT já deve ir no cabeçalho graças ao AuthService
                return await _httpClient.GetFromJsonAsync<List<ProdutoDTO>>("api/FornecedorProdutos")
                       ?? new List<ProdutoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter produtos: {ex.Message}");
                return new List<ProdutoDTO>();
            }
        }

        // 2. Criar Produto
        public async Task<bool> CriarProduto(ProdutoCreateDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/FornecedorProdutos", dto);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                var statusCode = (int)response.StatusCode; // Obtém o código (ex: 400, 500)

                // Se a mensagem vier vazia, usamos o status code para dar uma pista
                if (string.IsNullOrWhiteSpace(erro))
                {
                    throw new Exception($"Erro API ({statusCode}): {response.ReasonPhrase}");
                }

                throw new Exception($"Erro API ({statusCode}): {erro}");
            }
            return true;
        }
        // 3. Editar Produto
        public async Task<bool> EditarProduto(int id, ProdutoCreateDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/FornecedorProdutos/{id}", dto);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                var statusCode = (int)response.StatusCode;

                if (string.IsNullOrWhiteSpace(erro))
                {
                    throw new Exception($"Erro API ({statusCode}): {response.ReasonPhrase}");
                }

                throw new Exception($"Erro API ({statusCode}): {erro}");
            }
            return true;
        }

        // 4. Apagar Produto
        public async Task<bool> ApagarProduto(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/FornecedorProdutos/{id}");
            return response.IsSuccessStatusCode;
        }

        // 5. Histórico de Vendas
        public async Task<List<VendaFornecedorDTO>> GetMinhasVendas()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<VendaFornecedorDTO>>("api/FornecedorProdutos/vendas")
                       ?? new List<VendaFornecedorDTO>();
            }
            catch
            {
                return new List<VendaFornecedorDTO>();
            }
        }
    }
}