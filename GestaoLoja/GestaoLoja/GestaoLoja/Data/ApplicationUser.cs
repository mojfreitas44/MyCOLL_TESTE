using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GestaoLoja.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string Nome { get; set; } = string.Empty;

        [PersonalData]
        public string Apelido { get; set; } = string.Empty;

        [PersonalData]
        public long NIF { get; set; } // Mudámos para LONG

        [PersonalData]
        public string Telemovel { get; set; } = string.Empty;

        // Morada Detalhada
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

        public string Estado { get; set; } = "Pendente";
        public DateTime DataRegisto { get; set; } = DateTime.UtcNow;
    }
}