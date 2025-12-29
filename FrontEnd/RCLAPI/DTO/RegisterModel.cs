using System.ComponentModel.DataAnnotations;

namespace RCLAPI.DTO
{
    public class RegisterModel
    {
        // Dados de Login
        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Password é obrigatória")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "As passwords não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Dados Pessoais
        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Apelido é obrigatório")]
        public string Apelido { get; set; } = string.Empty;

        [Required(ErrorMessage = "O NIF é obrigatório")]
        public long NIF { get; set; }

        public string Telemovel { get; set; } = string.Empty;

        // Morada Completa
        [Required] public string Rua { get; set; } = string.Empty;
        [Required] public string Localidade { get; set; } = string.Empty;
        [Required] public string CodigoPostal { get; set; } = string.Empty;
        [Required] public string Cidade { get; set; } = string.Empty;
        [Required] public string Pais { get; set; } = string.Empty;
    }
}