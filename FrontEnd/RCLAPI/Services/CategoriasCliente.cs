using RCLAPI.DTO;
using System.Net.Http.Json;
using System.Text.Json;

namespace RCLAPI.Services
{
    public class CategoriasCliente
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public CategoriasCliente(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<CategoriaDTO>> GetCategorias()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CategoriaDTO>>("api/Categorias", _options);
                return response ?? new List<CategoriaDTO>();
            }
            catch
            {
                return new List<CategoriaDTO>();
            }
        }
    }
}