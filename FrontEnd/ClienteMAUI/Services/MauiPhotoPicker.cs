using Microsoft.Maui.ApplicationModel; // Importante para MainThread
using Microsoft.Maui.Media;
using RCLAPI.Services;

namespace ClienteMAUI.Services
{
    public class MauiPhotoPicker : PhotoPicker
    {
        public async Task<string?> PickPhotoBase64Async()
        {
            // Executa na Thread Principal para prevenir crashes (Erro 0x80000003 no Windows)
            return await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                    {
                        // Abre a galeria nativa
                        var photo = await MediaPicker.Default.PickPhotoAsync();

                        if (photo != null)
                        {
                            using var stream = await photo.OpenReadAsync();
                            using var ms = new MemoryStream();
                            await stream.CopyToAsync(ms);

                            // Devolve a imagem em Base64 para mostrar no Blazor
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MauiPhotoPicker] Erro: {ex.Message}");
                }

                return null;
            });
        }
    }
}