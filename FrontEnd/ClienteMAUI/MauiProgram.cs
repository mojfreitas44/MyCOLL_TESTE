using Microsoft.Extensions.Logging;
using RCLAPI;
using RCLAPI.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization; // <--- NOVO: Precisas disto

namespace ClienteMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // --- INÍCIO DAS CONFIGURAÇÕES NOVAS ---

            // 1. Configurar o HttpClient com o endereço da API (Já tinhas, mas confirma)
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(AppConfig.BaseUrl) });

            // 2. Registar o LocalStorage (Para guardar o Token)
            builder.Services.AddBlazoredLocalStorage();

            // 3. OBRIGATÓRIO: Sistema de Autorização do Blazor
            // (Sem isto, o <CascadingAuthenticationState> faz a app crashar)
            builder.Services.AddAuthorizationCore();

            // 4. Registar o nosso Serviço de Autenticação Personalizado
            builder.Services.AddScoped<AuthenticationStateProvider, AuthService>();

            // (Truque para poderes injetar tanto 'AuthenticationStateProvider' como 'AuthService')
            builder.Services.AddScoped<AuthService>(provider =>
                (AuthService)provider.GetRequiredService<AuthenticationStateProvider>());

            // 5. Registar o Serviço de Produtos (Para a página de Catálogo funcionar)
            builder.Services.AddScoped<ProdutosCliente>();

            // 6. Registar o Serviço de Categorias (Para a página de Catálogo funcionar)
            builder.Services.AddScoped<CategoriasCliente>();

            // --- FIM DAS CONFIGURAÇÕES ---

            return builder.Build();
        }
    }
}