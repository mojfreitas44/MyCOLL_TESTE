using RCLAPI.DTO;
using System.Net.Http.Json;

namespace RCLAPI.Services
{
    public class EncomendasCliente
    {
        private readonly HttpClient _httpClient;

        public EncomendasCliente(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ModoEntregaDTO>> GetModosEntrega()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ModoEntregaDTO>>("api/ModoEntrega")
                       ?? new List<ModoEntregaDTO>();
            }
            catch
            {
                return new List<ModoEntregaDTO>();
            }
        }

        public async Task<bool> SubmeterEncomenda(CheckoutDTO checkout)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("api/Encomendas", checkout);
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}