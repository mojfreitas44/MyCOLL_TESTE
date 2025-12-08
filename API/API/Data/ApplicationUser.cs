using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    // Igual ao do GestaoLoja, mas com namespace da API
    public class ApplicationUser : IdentityUser
    {
        public string? Nome { get; set; }
        public string? NIF { get; set; }
        public string? Morada { get; set; }
        public string Estado { get; set; } = "Pendente";
    }
}