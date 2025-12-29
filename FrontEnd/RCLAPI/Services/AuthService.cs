using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using RCLAPI.DTO;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace RCLAPI.Services
{
    public class AuthService : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Tenta ler o token da memória
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (string.IsNullOrEmpty(token))
                {
                    // Se não houver token, devolve "anónimo" (não logado)
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Se houver token, configura o pedido HTTP
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Devolve o utilizador autenticado
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
            }
            catch
            {
                // <--- O PARAQUEDAS: Se der QUALQUER erro (ex: storage vazio), assume que não está logado
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task<LoginResponse> Login(LoginModel loginModel)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("api/Utilizadores/login", loginModel);

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<LoginResponse>();

                    if (response != null && !string.IsNullOrEmpty(response.AccessToken))
                    {
                        await _localStorage.SetItemAsync("authToken", response.AccessToken);
                        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                        response.Sucesso = true;
                        return response;
                    }
                }

                return new LoginResponse { Sucesso = false, MensagemErro = "Login falhou. Credenciais erradas." };
            }
            catch (Exception ex)
            {
                return new LoginResponse { Sucesso = false, MensagemErro = "Erro na API: " + ex.Message };
            }
        }

        public async Task<bool> Registar(RegisterModel registoModel)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("api/Utilizadores/register", registoModel);
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs == null)
            {
                return Enumerable.Empty<Claim>();
            }

            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? ""));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}