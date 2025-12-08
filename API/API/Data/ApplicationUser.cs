using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? Nome { get; set; }

        [PersonalData]
        public string? NIF { get; set; }

        [PersonalData]
        public string? Morada { get; set; }

        // Regra de Neg�cio: Utilizadores entram como "Pendente" at� serem aprovados
        public string Estado { get; set; } = "Pendente";
    }
}