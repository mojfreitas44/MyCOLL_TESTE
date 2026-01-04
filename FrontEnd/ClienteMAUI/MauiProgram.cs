using Microsoft.Extensions.Logging;
using RCLAPI;
using RCLAPI.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ClienteMAUI.Services;

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

            // 1. Configurar o HttpClient com o endereço da API 
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(AppConfig.BaseUrl) });

            // 2. Registar o LocalStorage (Para guardar o Token)
            builder.Services.AddBlazoredLocalStorage();

            // 3. Sistema de Autorização do Blazor
            // (Sem isto, o <CascadingAuthenticationState> faz a app crashar)
            builder.Services.AddAuthorizationCore();

            // 4. Registar o Serviço de Autenticação Personalizado
            builder.Services.AddScoped<AuthenticationStateProvider, AuthService>();

            // (Injetar tanto 'AuthenticationStateProvider' como 'AuthService')
            builder.Services.AddScoped<AuthService>(provider =>
                (AuthService)provider.GetRequiredService<AuthenticationStateProvider>());

            // 5. Registar o Serviço de Produtos
            builder.Services.AddScoped<ProdutosCliente>();

            // 6. Registar o Serviço de Categorias 
            builder.Services.AddScoped<CategoriasCliente>();

            // 7. Registar o Serviço do Carrinho de Compras
            builder.Services.AddScoped<CarrinhoService>();

            // 8. Registar o Serviço de Encomendas
            builder.Services.AddScoped<EncomendasCliente>();

            // 9. Registar o Serviço de Fornecedores
            builder.Services.AddScoped<FornecedorService>();

            // 10. Registar o Serviço de Favoritos
            builder.Services.AddScoped<FavoritosService>();

            // 11. Registar o Serviço de Upload de Fotos
            builder.Services.AddSingleton<PhotoPicker, MauiPhotoPicker>();

            // --- FIM DAS CONFIGURAÇÕES ---

            return builder.Build();
        }
    }
}