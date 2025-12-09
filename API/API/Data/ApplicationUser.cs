using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class ApplicationUser : IdentityUser
    {
        // Dados Pessoais
        [PersonalData]
        public string Nome { get; set; } = string.Empty;

        [PersonalData]
        public string Apelido { get; set; } = string.Empty;

        [PersonalData]
        public long NIF { get; set; } // Agora é long para combinar com o AuthController

        [PersonalData]
        public string Telemovel { get; set; } = string.Empty;

        // Dados de Morada
        [PersonalData]
        public string Rua { get; set; } = string.Empty;

        [PersonalData]
        public string Localidade { get; set; } = string.Empty;

        [PersonalData]
        public string CodigoPostal { get; set; } = string.Empty;

        [PersonalData]
        public string Cidade { get; set; } = string.Empty;

        [PersonalData]
        public string Pais { get; set; } = string.Empty;

        // Dados de Sistema
        public string Estado { get; set; } = "Pendente";
        public DateTime DataRegisto { get; set; } = DateTime.UtcNow;
    }
}