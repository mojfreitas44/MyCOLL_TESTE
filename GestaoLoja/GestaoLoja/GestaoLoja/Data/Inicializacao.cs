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
        /// Garante que as Roles e o Administrador existem.
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
            var emailAdmin = "admin@mycoll.pt"; // O teu email de admin

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
                    Estado = "Ativo"
                };
                await userManager.CreateAsync(novoAdmin, "Admin123!");
                await userManager.AddToRoleAsync(novoAdmin, Roles.Administrador);
            }
        }
        /*
        /// <summary>
        /// Cria categorias base apenas se a tabela estiver vazia.
        /// (Podes apagar este método se preferires gerir categorias manualmente)
        /// </summary>
        public static async Task SeedCategoriasPadrao(ApplicationDbContext db)
        {
            if (await db.Categorias.AnyAsync()) return;

            db.Categorias.AddRange(
                new Categoria { Nome = "Moedas", Descricao = "Numismática" },
                new Categoria { Nome = "Selos", Descricao = "Filatelia" },
                new Categoria { Nome = "Complementos", Descricao = "Material de preservação" }
            );

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Cria uma encomenda de teste para desenvolvimento.
        /// (Podes apagar este método quando já não precisares de testes)
        /// </summary>
        public static async Task SeedEncomendaTeste(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            if (await db.Encomendas.AnyAsync()) return;

            var cliente = await userManager.FindByEmailAsync("admin@mycoll.pt");
            var produto = await db.Produtos.FirstOrDefaultAsync();

            if (cliente == null || produto == null) return;

            var encomenda = new Encomenda
            {
                Data = DateTime.Now,
                ValorTotal = produto.PrecoVenda,
                Estado = "Pendente",
                MoradaEnvio = "Loja (Teste)",
                MetodoPagamento = "Dinheiro",
                MetodoEntrega = "Levantamento",
                ClienteId = cliente.Id,
                Itens = new List<EncomendaItem>
                {
                    new EncomendaItem
                    {
                        ProdutoId = produto.Id,
                        Quantidade = 1,
                        PrecoUnitario = produto.PrecoVenda
                    }
                }
            };

            db.Encomendas.Add(encomenda);
            await db.SaveChangesAsync();
        }*/
    }
}