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
    [Authorize(Roles = "Fornecedor")] // 🔒 Só Fornecedores entram aqui!
    public class FornecedorProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _repo;
        private readonly IEncomendaRepository _encomendaRepo; // <--- NOVO: Para ver as vendas
        private readonly UserManager<ApplicationUser> _userManager;

        // Atualizámos o construtor para receber o IEncomendaRepository
        public FornecedorProdutosController(
            IProdutoRepository repo,
            IEncomendaRepository encomendaRepo,
            UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _encomendaRepo = encomendaRepo;
            _userManager = userManager;
        }

        // 1. LISTAR MEUS PRODUTOS
        // GET: api/FornecedorProdutos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetMeusProdutos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var produtos = await _repo.GetMeusProdutosAsync(userId!);

            var dtos = produtos.Select(p => new ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                PrecoVenda = p.PrecoVenda,
                Condicao = p.Condicao,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.Categoria?.Nome,
                Imagem = p.Imagem,
                Disponibilidade = p.Estado
            });

            return Ok(dtos);
        }

        // 2. CRIAR PRODUTO
        // POST: api/FornecedorProdutos
        [HttpPost]
        public async Task<ActionResult> Criar([FromBody] ProdutoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            byte[]? imagemBytes = null;
            if (!string.IsNullOrEmpty(dto.ImagemBase64))
            {
                try { imagemBytes = Convert.FromBase64String(dto.ImagemBase64); } catch { }
            }

            var novoProduto = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                PrecoBase = dto.PrecoVenda,
                PrecoVenda = dto.PrecoVenda,
                Stock = dto.Stock,
                Condicao = dto.Condicao,
                CategoriaId = dto.CategoriaId,
                FornecedorId = userId,
                Imagem = imagemBytes,
                ParaVenda = true,
                Estado = "Pendente"
            };

            await _repo.CriarProdutoAsync(novoProduto);

            return Ok(new { Message = "Produto criado com sucesso! Aguarda aprovação do administrador." });
        }

        // 3. EDITAR PRODUTO
        // PUT: api/FornecedorProdutos/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Editar(int id, [FromBody] ProdutoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await _repo.SouDonoDoProduto(id, userId!))
            {
                return Forbid();
            }

            var produto = await _repo.GetByIdAsync(id);
            if (produto == null) return NotFound();

            if (dto.Stock < produto.Stock)
            {
                return BadRequest($"Erro: Não pode reduzir o stock manualmente (Stock Atual: {produto.Stock}). Apenas vendas reduzem o stock.");
            }

            produto.Nome = dto.Nome;
            produto.Descricao = dto.Descricao;
            produto.PrecoBase = dto.PrecoVenda;
            produto.PrecoVenda = dto.PrecoVenda;
            produto.Stock = dto.Stock;
            produto.Condicao = dto.Condicao;
            produto.CategoriaId = dto.CategoriaId;

            if (!string.IsNullOrEmpty(dto.ImagemBase64))
            {
                try { produto.Imagem = Convert.FromBase64String(dto.ImagemBase64); } catch { }
            }

            produto.Estado = "Pendente";

            await _repo.AtualizarProdutoAsync(produto);

            return Ok(new { Message = "Produto atualizado. O estado voltou a Pendente para reavaliação." });
        }

        // 4. APAGAR PRODUTO
        // DELETE: api/FornecedorProdutos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Apagar(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await _repo.SouDonoDoProduto(id, userId!)) return Forbid();

            await _repo.ApagarProdutoAsync(id);
            return NoContent();
        }

        // 5. HISTÓRICO DE VENDAS (NOVO!) 💰
        // GET: api/FornecedorProdutos/vendas
        [HttpGet("vendas")]
        public async Task<ActionResult<IEnumerable<VendaFornecedorDTO>>> GetMinhasVendas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Vai buscar as vendas deste fornecedor específico
            var vendas = await _encomendaRepo.GetVendasDoFornecedor(userId!);

            return Ok(vendas);
        }
    }
}