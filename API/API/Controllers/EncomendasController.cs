using API.DTO;
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
    [Authorize] // Só clientes logados podem fazer encomendas
    public class EncomendasController : ControllerBase
    {
        private readonly IEncomendaRepository _encomendaRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EncomendasController(IEncomendaRepository encomendaRepository, UserManager<ApplicationUser> userManager)
        {
            _encomendaRepository = encomendaRepository;
            _userManager = userManager;
        }

        // POST: api/Encomendas
        // Finalizar Compra (Checkout)
        [HttpPost]
        public async Task<ActionResult<EncomendaDTO>> CriarEncomenda([FromBody] CheckoutDto checkoutDto)
        {
            // 1. Quem é o cliente?
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // 2. O cliente está ativo?
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Estado != "Ativo")
                return StatusCode(403, "Conta não ativa.");

            try
            {
                // 3. Criar a encomenda (O repositório faz o trabalho sujo)
                var encomenda = await _encomendaRepository.CriarEncomenda(userId, checkoutDto);

                // 4. Retornar os dados da encomenda criada
                return Ok(new EncomendaDTO
                {
                    Id = encomenda.Id,
                    Data = encomenda.Data,
                    ValorTotal = encomenda.ValorTotal,
                    Estado = encomenda.Estado,
                    MoradaEnvio = encomenda.MoradaEnvio ?? "",
                    MetodoPagamento = encomenda.MetodoPagamento ?? "",
                    MetodoEntrega = encomenda.MetodoEntrega ?? "",
                    Itens = encomenda.Itens.Select(i => new ItemEncomendaDTO
                    {
                        ProdutoNome = i.Produto?.Nome ?? "Produto Desconhecido",
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                // Ex: "Carrinho vazio"
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Encomendas
        // Ver histórico de compras (Meus Pedidos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EncomendaDTO>>> GetMinhasEncomendas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var encomendas = await _encomendaRepository.GetEncomendasDoCliente(userId);

            // Converter para DTO
            var dtos = encomendas.Select(e => new EncomendaDTO
            {
                Id = e.Id,
                Data = e.Data,
                ValorTotal = e.ValorTotal,
                Estado = e.Estado,
                MoradaEnvio = e.MoradaEnvio ?? "",
                MetodoPagamento = e.MetodoPagamento ?? "",
                MetodoEntrega = e.MetodoEntrega ?? "",
                // No histórico geral não precisamos de mostrar os itens todos,
                // mas podemos mostrar o número de itens ou o primeiro produto.
                Itens = e.Itens.Select(i => new ItemEncomendaDTO
                {
                    ProdutoNome = i.Produto?.Nome ?? "Produto",
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            });

            return Ok(dtos);
        }

        // GET: api/Encomendas/5
        // Ver detalhes de uma encomenda específica
        [HttpGet("{id}")]
        public async Task<ActionResult<EncomendaDTO>> GetDetalhes(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var encomenda = await _encomendaRepository.GetDetalhesEncomenda(userId, id);

            if (encomenda == null) return NotFound("Encomenda não encontrada.");

            return Ok(new EncomendaDTO
            {
                Id = encomenda.Id,
                Data = encomenda.Data,
                ValorTotal = encomenda.ValorTotal,
                Estado = encomenda.Estado,
                MoradaEnvio = encomenda.MoradaEnvio ?? "",
                MetodoPagamento = encomenda.MetodoPagamento ?? "",
                MetodoEntrega = encomenda.MetodoEntrega ?? "",
                Itens = encomenda.Itens.Select(i => new ItemEncomendaDTO
                {
                    ProdutoNome = i.Produto?.Nome ?? "Produto Removido",
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            });
        }
    }
}