using GestaoLoja.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoLoja.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        // Podes adicionar aqui Encomendas, Carrinho, etc. mais tarde, tal como no WebCar

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Exemplo de configuração adicional se necessário (ex: definir precisão decimal se o atributo não funcionar)
            builder.Entity<Produto>()
                .Property(p => p.PrecoBase)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Produto>()
                .Property(p => p.PrecoVenda)
                .HasColumnType("decimal(18,2)");
        }
    }
}