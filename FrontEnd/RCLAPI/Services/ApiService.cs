using System.Net.Http.Json;
using RCLAPI;
using RCLAPI.DTO;

namespace RCLAPI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Verifica se o AppConfig.BaseUrl está correto no outro ficheiro
            _httpClient.BaseAddress = new Uri(AppConfig.BaseUrl);
        }

        public async Task<LoginResult?> Login(UtilizadorLoginModel loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/utilizadores/login", loginModel);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResult>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Register(RegisterModel registerModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/utilizadores/register", registerModel);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}