using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RCLAPI.DTO
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password é obrigatória")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        // Mapeia exatamente o JSON que a tua API devolve
        [JsonPropertyName("accesstoken")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("utilizadorid")]
        public string UtilizadorId { get; set; } = string.Empty;

        [JsonPropertyName("utilizadornome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        // Campos auxiliares para o frontend (não vêm da API)
        public bool Sucesso { get; set; } = true;
        public string MensagemErro { get; set; } = string.Empty;
    }
}