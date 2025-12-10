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
                FornecedorNome = p.Fornecedor?.Nome,
                Imagem = p.Imagem,

                Disponibilidade = p.Stock <= 0 ? "Esgotado" :
                                  p.Stock < 5 ? "Últimas Unidades!" : "Em Stock"
            });

            return Ok(dtos);
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
                FornecedorNome = p.Fornecedor?.Nome,
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
    }
}