using GestaoLoja.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoLoja.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        public DbSet<ModoEntrega> ModosEntrega { get; set; }

        public DbSet<Encomenda> Encomendas { get; set; }

        public DbSet<CarrinhoCompras> CarrinhoCompras { get; set; }

        public DbSet<Favorito> Favoritos { get; set; }

        // Configurações adicionais do modelo
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Produto>()
                .Property(p => p.PrecoBase)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Produto>()
                .Property(p => p.PrecoVenda)
                .HasColumnType("decimal(18,2)");
        }
    }
}