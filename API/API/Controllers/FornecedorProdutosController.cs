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
        private readonly UserManager<ApplicationUser> _userManager;

        public FornecedorProdutosController(IProdutoRepository repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        // 1. LISTAR MEUS PRODUTOS
        // GET: api/FornecedorProdutos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetMeusProdutos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Busca apenas os produtos deste fornecedor
            var produtos = await _repo.GetMeusProdutosAsync(userId!);

            // Converter para DTO
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
                // Mostra o estado real (Pendente/Ativo) ao fornecedor
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

            // Converter imagem Base64 (texto) para bytes
            byte[]? imagemBytes = null;
            if (!string.IsNullOrEmpty(dto.ImagemBase64))
            {
                try { imagemBytes = Convert.FromBase64String(dto.ImagemBase64); } catch { }
            }

            var novoProduto = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,

                // Preços
                PrecoBase = dto.PrecoVenda, // O preço que ele define é o base
                PrecoVenda = dto.PrecoVenda, // Inicialmente igual (admin pode mudar)

                // Stock e Condição
                Stock = dto.Stock,
                Condicao = dto.Condicao, // "Novo" ou "Usado"

                // Dados Fixos
                CategoriaId = dto.CategoriaId,
                FornecedorId = userId,
                Imagem = imagemBytes,
                ParaVenda = true,

                // REGRA: Nasce sempre Pendente
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

            // A. Verificar se o produto pertence mesmo a este fornecedor
            if (!await _repo.SouDonoDoProduto(id, userId!))
            {
                return Forbid(); // 403: Proibido mexer no que não é teu
            }

            var produto = await _repo.GetByIdAsync(id);
            if (produto == null) return NotFound();

            // B. REGRA DE STOCK: Fornecedor só pode aumentar, nunca diminuir
            if (dto.Stock < produto.Stock)
            {
                return BadRequest($"Erro: Não pode reduzir o stock manualmente (Stock Atual: {produto.Stock}). Apenas vendas reduzem o stock.");
            }

            // C. Atualizar Dados
            produto.Nome = dto.Nome;
            produto.Descricao = dto.Descricao;
            produto.PrecoBase = dto.PrecoVenda;
            produto.PrecoVenda = dto.PrecoVenda; // Reinicia o preço de venda
            produto.Stock = dto.Stock;
            produto.Condicao = dto.Condicao;
            produto.CategoriaId = dto.CategoriaId;

            // Atualizar imagem só se vier uma nova
            if (!string.IsNullOrEmpty(dto.ImagemBase64))
            {
                try { produto.Imagem = Convert.FromBase64String(dto.ImagemBase64); } catch { }
            }

            // D. REGRA: Voltar a Pendente após edição
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

            // Verificar dono
            if (!await _repo.SouDonoDoProduto(id, userId!)) return Forbid();

            await _repo.ApagarProdutoAsync(id);
            return NoContent();
        }
    }
}