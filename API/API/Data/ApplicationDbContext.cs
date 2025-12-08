using API.Entities; // Vamos criar isto no próximo passo
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tens de adicionar aqui TODAS as tuas tabelas
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        // public DbSet<Encomenda> Encomendas { get; set; } // Descomenta quando criares a classe
        // public DbSet<ModoEntrega> ModosEntrega { get; set; }
    }
}