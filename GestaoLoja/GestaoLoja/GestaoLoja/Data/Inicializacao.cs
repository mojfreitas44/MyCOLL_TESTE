using GestaoLoja.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestaoLoja.Data
{
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Funcionario = "Funcionario";
        public const string Fornecedor = "Fornecedor";
        public const string Cliente = "Cliente";
    }

    public static class Inicializacao
    {
        /// <summary>
        /// Método Principal: Garante que o sistema tem Roles e um Administrador para entrar.
        /// </summary>
        public static async Task CriaDadosIniciais(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Criar Roles (Perfis) se não existirem
            string[] roles = [Roles.Administrador, Roles.Funcionario, Roles.Fornecedor, Roles.Cliente];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Garantir que o Administrador existe e está Ativo
            // ATENÇÃO: Confirma se este é o email que queres usar como Super Admin
            var emailAdmin = "admin@mycoll.pt";

            var userAdmin = await userManager.FindByEmailAsync(emailAdmin);
            if (userAdmin != null)
            {
                // Se já existe, garante permissões e estado
                if (!await userManager.IsInRoleAsync(userAdmin, Roles.Administrador))
                {
                    await userManager.AddToRoleAsync(userAdmin, Roles.Administrador);
                }

                if (userAdmin.Estado != "Ativo")
                {
                    userAdmin.Estado = "Ativo";
                    await userManager.UpdateAsync(userAdmin);
                }
            }
            else
            {
                // Se não existe, cria de raiz
                var novoAdmin = new ApplicationUser
                {
                    UserName = emailAdmin,
                    Email = emailAdmin,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Estado = "Ativo",
                    Nome = "Administrador Principal"
                };
                await userManager.CreateAsync(novoAdmin, "Admin123!");
                await userManager.AddToRoleAsync(novoAdmin, Roles.Administrador);
            }
        }

        /// <summary>
        /// Opcional: Cria as categorias base do negócio (Moedas, Selos, etc.)
        /// </summary>
        public static async Task SeedCategoriasPadrao(ApplicationDbContext db)
        {
            // Se já existirem categorias, não faz nada (respeita os dados reais)
            if (await db.Categorias.AnyAsync()) return;

            db.Categorias.AddRange(
                new Categoria { Nome = "Moedas", Descricao = "Numismática antiga e moderna" },
                new Categoria { Nome = "Selos", Descricao = "Filatelia de todo o mundo" },
                new Categoria { Nome = "Carteiras de Fósforos", Descricao = "Filumenismo" },
                new Categoria { Nome = "Pacotes de Açúcar", Descricao = "Perifilía" },
                new Categoria { Nome = "Complementos", Descricao = "Álbuns, lupas e material de preservação" }
            );

            await db.SaveChangesAsync();
        }
    }
}