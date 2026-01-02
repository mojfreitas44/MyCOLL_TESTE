using Microsoft.AspNetCore.Mvc;
using API.DTO;
using API.Repositories;
using Microsoft.AspNetCore.Identity; // 1. gerir Users
using API.Data; // 2. ApplicationUser

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly UserManager<ApplicationUser> _userManager; // 3. Injeção do UserManager

        public ProdutosController(IProdutoRepository produtoRepository, UserManager<ApplicationUser> userManager)
        {
            _produtoRepository = produtoRepository;
            _userManager = userManager;
        }

        // GET: api/Produtos
        // Serve para a Página Inicial, Pesquisa e Filtros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] string? pesquisa, [FromQuery] int? categoriaId)
        {
            var produtos = await _produtoRepository.GetAllAsync(pesquisa, categoriaId);

            // Vamos buscar os IDs de todos os Admins e Funcionários para verificar rapidamente
            var admins = await _userManager.GetUsersInRoleAsync("Administrador");
            var funcionarios = await _userManager.GetUsersInRoleAsync("Funcionario");

            // Criamos um conjunto (HashSet) com os IDs oficiais para pesquisa rápida
            var idsOficiais = admins.Concat(funcionarios).Select(u => u.Id).ToHashSet();

            var produtosDto = produtos.Select(p => {
                // Verificamos se o fornecedor deste produto está na lista de oficiais
                bool eOficial = p.Fornecedor != null && idsOficiais.Contains(p.Fornecedor.Id);

                return new ProdutoDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    PrecoVenda = p.PrecoVenda,
                    Imagem = p.Imagem,
                    Condicao = p.Condicao,
                    ParaVenda = p.ParaVenda,
                    Estado = p.Estado,
                    Stock = p.Stock,
                    CategoriaId = p.CategoriaId,
                    CategoriaNome = p.Categoria?.Nome,

                    // Se for oficial, força o nome. Se não, usa a lógica antiga.
                    FornecedorNome = eOficial
                        ? "Produto Oficial MyCOLL"
                        : (!string.IsNullOrEmpty(p.Fornecedor?.Nome) ? p.Fornecedor.Nome : (p.Fornecedor?.UserName ?? "Produto Oficial")),

                    Disponibilidade = p.Stock <= 0 ? "Esgotado" :
                      p.Stock <= 5 ? "Últimas Unidades" :
                      "Em Stock"
                };
            }).ToList();

            return Ok(produtosDto);
        }

        // GET: api/Produtos/destaque
        [HttpGet("destaque")]
        public async Task<ActionResult<ProdutoDTO>> GetDestaque()
        {
            var produto = await _produtoRepository.GetProdutoDestaqueAsync();

            if (produto == null)
            {
                return NotFound("Nenhum produto em destaque encontrado.");
            }

            // Verificar se é oficial (para um só produto, podemos verificar direto)
            bool eOficial = false;
            if (produto.Fornecedor != null)
            {
                var roles = await _userManager.GetRolesAsync(produto.Fornecedor);
                eOficial = roles.Contains("Administrador") || roles.Contains("Funcionario");
            }

            var dto = new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                PrecoVenda = produto.PrecoVenda,
                Imagem = produto.Imagem,
                Condicao = produto.Condicao,
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome,
                ParaVenda = produto.ParaVenda,
                Stock = produto.Stock,
                Estado = produto.Estado,
                FornecedorNome = eOficial
                    ? "Produto Oficial MyCOLL"
                    : (!string.IsNullOrEmpty(produto.Fornecedor?.Nome) ? produto.Fornecedor.Nome : (produto.Fornecedor?.UserName ?? "Produto Oficial")),
                Disponibilidade = produto.Stock <= 0 ? "Esgotado" :
                  produto.Stock <= 5 ? "Últimas Unidades" :
                  "Em Stock"
            };

            return Ok(dto);
        }

        // GET: api/Produtos/5
        // Serve para a página de detalhes do produto
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            // Verificar se é oficial
            bool eOficial = false;
            if (produto.Fornecedor != null)
            {
                var roles = await _userManager.GetRolesAsync(produto.Fornecedor);
                eOficial = roles.Contains("Administrador") || roles.Contains("Funcionario");
            }

            var dto = new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                PrecoVenda = produto.PrecoVenda,
                Imagem = produto.Imagem,
                Condicao = produto.Condicao,
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome,
                ParaVenda = produto.ParaVenda,
                Stock = produto.Stock,
                Estado = produto.Estado,
                FornecedorNome = eOficial
                   ? "Produto Oficial MyCOLL"
                   : (!string.IsNullOrEmpty(produto.Fornecedor?.Nome) ? produto.Fornecedor.Nome : (produto.Fornecedor?.UserName ?? "Produto Oficial")),
                Disponibilidade = produto.Stock <= 0 ? "Esgotado" :
                  produto.Stock <= 5 ? "Últimas Unidades" :
                  "Em Stock"
            };

            return Ok(dto);
        }
    }
}