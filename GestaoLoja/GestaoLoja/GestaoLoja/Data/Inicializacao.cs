using GestaoLoja.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestaoLoja.Data
{
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Funcionario = "Funcionario"; // Sem acento, para evitar erros
        public const string Fornecedor = "Fornecedor";
        public const string Cliente = "Cliente";
    }

    public static class Inicializacao
    {
        public static async Task CriaDadosIniciais(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Garantir que as Roles CERTAS existem na BD
            string[] roles = [Roles.Administrador, Roles.Funcionario, Roles.Fornecedor, Roles.Cliente];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. LIMPEZA AUTOMÁTICA: Resolver o problema do "Funcionário" com acento
            string roleErrada = "Funcionário";
            if (await roleManager.RoleExistsAsync(roleErrada))
            {
                // Encontrar quem tem a role errada
                var usersComErro = await userManager.GetUsersInRoleAsync(roleErrada);

                foreach (var user in usersComErro)
                {
                    // Mover para a role certa (Funcionario sem acento)
                    if (!await userManager.IsInRoleAsync(user, Roles.Funcionario))
                    {
                        await userManager.AddToRoleAsync(user, Roles.Funcionario);
                    }
                    // Remover da errada
                    await userManager.RemoveFromRoleAsync(user, roleErrada);
                }

                // Apagar a role errada da base de dados para não aparecer mais nas dropdowns
                var roleToDelete = await roleManager.FindByNameAsync(roleErrada);
                if (roleToDelete != null)
                {
                    await roleManager.DeleteAsync(roleToDelete);
                }
            }

            // 3. Configurar o Teu Administrador
            var emailAdmin = "admin@mycoll.pt";

            var userAdmin = await userManager.FindByEmailAsync(emailAdmin);
            if (userAdmin != null)
            {
                // Se o user já existe, garantir que tem a Role de Admin
                if (!await userManager.IsInRoleAsync(userAdmin, Roles.Administrador))
                {
                    await userManager.AddToRoleAsync(userAdmin, Roles.Administrador);
                }

                // IMPORTANTE: Garantir que está "Ativo" para não ficares com AccessDenied
                if (userAdmin.Estado != "Ativo")
                {
                    userAdmin.Estado = "Ativo";
                    await userManager.UpdateAsync(userAdmin);
                }
            }
            else
            {
                // Se não existe, cria-o de raiz
                var novoAdmin = new ApplicationUser
                {
                    UserName = emailAdmin,
                    Email = emailAdmin,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Estado = "Ativo" // Cria logo como ativo
                };

                await userManager.CreateAsync(novoAdmin, "Admin123!");
                await userManager.AddToRoleAsync(novoAdmin, Roles.Administrador);
            }
        }

        public static async Task SeedCategoriasPadrao(ApplicationDbContext db)
        {
            // Se já existirem categorias, não faz nada
            if (await db.Categorias.AnyAsync()) return;

            // Adiciona categorias base do enunciado
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