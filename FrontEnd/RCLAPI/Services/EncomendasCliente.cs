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
        public async Task<List<EncomendaDTO>> GetMinhasEncomendas()
        {
            try
            {
                // O Controller usa "GET api/Encomendas" e descobre o user pelo Token
                return await _httpClient.GetFromJsonAsync<List<EncomendaDTO>>("api/Encomendas")
                       ?? new List<EncomendaDTO>();
            }
            catch
            {
                return new List<EncomendaDTO>();
            }
        }
        public async Task<bool> ConfirmarRececao(int encomendaId)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/Encomendas/{encomendaId}/confirmar-entrega", null);

                if (!response.IsSuccessStatusCode)
                {
                    // Opcional: Podes meter um breakpoint aqui para veres o 'ReasonPhrase'
                    var erro = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"ERRO API: {response.StatusCode} - {erro}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO DE REDE: {ex.Message}");
                return false;
            }
        }
    }
}