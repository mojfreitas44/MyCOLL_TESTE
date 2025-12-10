using API.DTO;
using API.Entities;
using API.Repositories;
using API.Data; // Para aceder ao ApplicationUser
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Obriga a estar logado (Token JWT)
    public class CarrinhoController : ControllerBase
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarrinhoController(ICarrinhoRepository carrinhoRepository, UserManager<ApplicationUser> userManager)
        {
            _carrinhoRepository = carrinhoRepository;
            _userManager = userManager;
        }

        // GET: api/Carrinho
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemCarrinhoDTO>>> GetCarrinho()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Pega o ID do Token
            if (userId == null) return Unauthorized();

            var itens = await _carrinhoRepository.GetCarrinhoDoCliente(userId);

            // Converter para DTO
            var dto = itens.Select(i => new ItemCarrinhoDTO
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                Nome = i.Produto?.Nome ?? "Produto Removido",
                Preco = i.Produto?.PrecoVenda ?? 0
            });

            return Ok(dto);
        }

        // POST: api/Carrinho (Body: { produtoId: 1, quantidade: 2 })
        [HttpPost]
        public async Task<IActionResult> Adicionar([FromBody] ItemCarrinhoDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Verificar se user está ativo
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Estado != "Ativo")
                return StatusCode(403, "Conta não ativa.");

            try
            {
                await _carrinhoRepository.AdicionarItem(userId, dto.ProdutoId, dto.Quantidade);
                return Ok("Adicionado ao carrinho");
            }
            catch (Exception ex)
            {
                // Devolve erro 400 se o stock for insuficiente
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Carrinho/produto/1/quantidade (Body: 5)
        [HttpPut("produto/{produtoId}/quantidade")]
        public async Task<IActionResult> AtualizarQtd(int produtoId, [FromBody] int quantidade)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                await _carrinhoRepository.AtualizarQuantidade(userId, produtoId, quantidade);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Devolve erro 400 se o stock for insuficiente
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Carrinho/produto/1
        [HttpDelete("produto/{produtoId}")]
        public async Task<IActionResult> RemoverItem(int produtoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _carrinhoRepository.RemoverItem(userId, produtoId);
            return NoContent();
        }

        // DELETE: api/Carrinho/esvaziar
        [HttpDelete("esvaziar")]
        public async Task<IActionResult> Esvaziar()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _carrinhoRepository.LimparCarrinho(userId);
            return NoContent();
        }
    }
}