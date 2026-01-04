using API.Entities; 
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

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<CarrinhoCompras> CarrinhoCompras { get; set; }
        public DbSet<ModoEntrega> ModosEntrega { get; set; }        
        public DbSet<Encomenda> Encomendas { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }

    }
}