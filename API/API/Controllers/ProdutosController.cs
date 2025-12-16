using API.DTO;
using API.Entities;
using API.Repositories;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProdutosController(IProdutoRepository repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] string? pesquisa, [FromQuery] int? categoriaId)
        {
            var produtos = await _repo.GetAllAsync(pesquisa, categoriaId);

            var dtos = produtos.Select(p => new ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                PrecoVenda = p.PrecoVenda,
                Condicao = p.Condicao,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.Categoria?.Nome,
                FornecedorNome = p.Fornecedor?.Nome ?? p.Fornecedor?.UserName ?? "Desconhecido",
                Imagem = p.Imagem,

                Disponibilidade = p.Stock <= 0 ? "Esgotado" :
                                  p.Stock < 5 ? "Últimas Unidades!" : "Em Stock"
            });

            return Ok(dtos);
        }
        [HttpGet("destaque")]
        public async Task<ActionResult<ProdutoDTO>> GetDestaque()
        {
            var p = await _repo.GetProdutoDestaqueAsync();

            if (p == null) return NotFound("Não há produtos para destaque.");

            return Ok(new ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                PrecoVenda = p.PrecoVenda,
                Condicao = p.Condicao,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.Categoria?.Nome,
                FornecedorNome = p.Fornecedor?.Nome ?? p.Fornecedor?.UserName ?? "Desconhecido",
                Imagem = p.Imagem,
                Disponibilidade = CalcularDisponibilidade(p.Stock)
            });
        }
        // GET: api/Produtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound();

            return Ok(new ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,

                // AQUI TAMBÉM
                PrecoVenda = p.PrecoVenda,

                Condicao = p.Condicao,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.Categoria?.Nome,
                FornecedorNome = p.Fornecedor?.Nome ?? p.Fornecedor?.UserName ?? "Desconhecido",
                Imagem = p.Imagem
            });
        }

        // GET: api/Produtos/meus-produtos
        [Authorize(Roles = "Fornecedor")]
        [HttpGet("meus-produtos")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetMeusProdutos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var produtos = await _repo.GetMeusProdutosAsync(userId!);

            return Ok(produtos.Select(p => new ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                PrecoVenda = p.PrecoVenda, // E AQUI
                Condicao = p.Condicao
            }));
        }

        // POST: api/Produtos
        [Authorize(Roles = "Fornecedor")]
        [HttpPost]
        public async Task<ActionResult> CriarProduto([FromBody] ProdutoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            byte[]? imagemBytes = null;
            if (!string.IsNullOrEmpty(dto.ImagemBase64))
            {
                try { imagemBytes = Convert.FromBase64String(dto.ImagemBase64); }
                catch { /* Ignorar erro de imagem */ }
            }

            var novoProduto = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,

                PrecoVenda = dto.PrecoVenda,
                PrecoBase = dto.PrecoVenda, 
                Stock = 1, 
                           

                Condicao = dto.Condicao,
                CategoriaId = dto.CategoriaId,
                FornecedorId = userId,
                Imagem = imagemBytes,
                ParaVenda = true,
                Estado = "Pendente"
            };

            await _repo.CriarProdutoAsync(novoProduto);

            return Ok(new { Message = "Produto criado com sucesso!" });
        }
        // PUT: api/Produtos/5
        [Authorize(Roles = "Fornecedor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProduto(int id, [FromBody] ProdutoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Segurança: O produto é mesmo deste fornecedor?
            if (!await _repo.SouDonoDoProduto(id, userId!))
            {
                return Forbid(); // 403: Não é teu, não mexes.
            }
            // 2. Buscar o produto original à BD
            var produto = await _repo.GetByIdAsync(id);
            if (produto == null) return NotFound();

            // 3. Atualizar os dados
            produto.Nome = dto.Nome;
            produto.Descricao = dto.Descricao;

            // Regra: Atualiza Preço Base e Preço Venda fica igual (até o Admin mexer)
            produto.PrecoBase = dto.PrecoVenda;
            produto.PrecoVenda = dto.PrecoVenda;

            produto.Condicao = dto.Condicao;
            produto.CategoriaId = dto.CategoriaId;
            produto.ParaVenda = true; // Se editou, assume-se que quer vender

            // Atualizar imagem se vier uma nova
            if (!string.IsNullOrEmpty(dto.ImagemBase64))
            {
                try { produto.Imagem = Convert.FromBase64String(dto.ImagemBase64); }
                catch { /* Ignorar erro de conversão de imagem */ }
            }

            // 4. REGRA DE OURO: Voltar ao estado "Pendente"
            produto.Estado = "Pendente";

            // 5. Gravar
            await _repo.AtualizarProdutoAsync(produto);

            return Ok(new { Message = "Produto atualizado! Ficou Pendente a aguardar aprovação." });
        }


        // DELETE: api/Produtos/5
        [Authorize(Roles = "Fornecedor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> ApagarProduto(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await _repo.SouDonoDoProduto(id, userId!))
            {
                return Forbid();
            }

            await _repo.ApagarProdutoAsync(id);
            return NoContent();
        }
        private string CalcularDisponibilidade(int stock)
        {
            if (stock <= 0) return "Esgotado";
            if (stock < 5) return "Últimas Unidades!";
            return "Em Stock";
        }
    }
}