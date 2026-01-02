using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using RCLAPI.DTO;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace RCLAPI.Services
{
    public class AuthService : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager; // 1. Adicionado NavigationManager

        // 2. Injetamos o NavigationManager no construtor
        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (string.IsNullOrEmpty(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
            }
            catch
            {
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
                        var role = response.Role?.ToLower() ?? "";

                        if (role != "cliente" && role != "fornecedor")
                        {
                            return new LoginResponse
                            {
                                Sucesso = false,
                                MensagemErro = "Acesso negado. Apenas Clientes ou Fornecedores podem usar esta App."
                            };
                        }

                        // Se passou, guardamos o token e notificamos
                        await _localStorage.SetItemAsync("authToken", response.AccessToken);
                        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

                        response.Sucesso = true;
                        return response;
                    }
                }

                var msgErro = await result.Content.ReadAsStringAsync();
                msgErro = msgErro.Trim('"');
                if (string.IsNullOrEmpty(msgErro)) msgErro = "Login falhou.";

                return new LoginResponse { Sucesso = false, MensagemErro = msgErro };
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

            // Opcional: Redirecionar para home ou login após logout
            // _navigationManager.NavigateTo("login"); 
        }

        // --- MÉTODO ATUALIZADO COM DETEÇÃO DE EXPIRAÇÃO ---
        public async Task<(UserPerfilResponse? user, string error)> ObterPerfil()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(token))
                    return (null, "Token não encontrado.");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("api/Utilizadores/perfil");

                // SE OCORRER ERRO 401 (UNAUTHORIZED)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Token expirou ou é inválido.
                    // 1. Forçar Logout
                    await Logout();

                    // 2. Enviar para a página de Login
                    _navigationManager.NavigateTo("login");

                    return (null, "Sessão expirada. A redirecionar para login...");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    return (null, $"Erro API ({response.StatusCode}): {msg}");
                }

                var dados = await response.Content.ReadFromJsonAsync<UserPerfilResponse>();
                return (dados, "");
            }
            catch (Exception ex)
            {
                return (null, $"Erro de conexão: {ex.Message}");
            }
        }

        public async Task<bool> AtualizarPerfil(EditarPerfilModel model)
        {
            try
            {
                var result = await _httpClient.PutAsJsonAsync("api/Utilizadores/perfil", model);

                // Também podemos aplicar a lógica de auto-logout aqui
                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Logout();
                    _navigationManager.NavigateTo("login");
                    return false;
                }

                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public async Task<string> AlterarPassword(string passAtual, string passNova, string passConfirm)
        {
            var model = new { PasswordAtual = passAtual, NovaPassword = passNova, ConfirmarNovaPassword = passConfirm };
            var result = await _httpClient.PostAsJsonAsync("api/Utilizadores/alterar-password", model);

            if (result.IsSuccessStatusCode) return ""; // Sucesso (string vazia = sem erro)

            return await result.Content.ReadAsStringAsync(); // Retorna a mensagem de erro do servidor
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