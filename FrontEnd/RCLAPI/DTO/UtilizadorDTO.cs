using System.ComponentModel.DataAnnotations;

namespace RCLAPI.DTO
{
    public class UtilizadorLoginModel
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "A password é obrigatória")]
        public string Password { get; set; } = "";
    }

    public class RegisterModel
    {
        [Required]
        public string Nome { get; set; } = "";
        [Required]
        public string Apelido { get; set; } = "";
        [Required]
        public long NIF { get; set; }
        public string Telemovel { get; set; } = "";

        // Morada (agrupada)
        public string Rua { get; set; } = "";
        public string Localidade { get; set; } = "";
        public string CodigoPostal { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Pais { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }

    public class LoginResult
    {
        public string accesstoken { get; set; } = "";
        public string tokentype { get; set; } = "";
        public string utilizadorid { get; set; } = "";
        public string utilizadornome { get; set; } = "";
        public string role { get; set; } = "";
        public string estado { get; set; } = "";
    }
}