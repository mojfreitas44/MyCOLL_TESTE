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
        public DbSet<CarrinhoCompras> CarrinhoCompras { get; set; }
        public DbSet<ModoEntrega> ModoEntrega { get; set; }        
        public DbSet<Encomenda> Encomendas { get; set; } 

    }
}