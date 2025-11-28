using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GestaoLoja.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? Nome { get; set; }

        [PersonalData]
        public string? NIF { get; set; }

        [PersonalData]
        public string? Morada { get; set; }

        // Regra de Negócio: Utilizadores entram como "Pendente" até serem aprovados
        public string Estado { get; set; } = "Pendente";
    }
}