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
    [Authorize]
    public class EncomendasController : ControllerBase
    {
        private readonly IEncomendaRepository _encomendaRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EncomendasController(IEncomendaRepository encomendaRepository, UserManager<ApplicationUser> userManager)
        {
            _encomendaRepository = encomendaRepository;
            _userManager = userManager;
        }

        // 1. CRIAR ENCOMENDA
        [HttpPost]
        // Se tiveres problemas com roles no checkout, podes remover esta linha também,
        // mas no checkout convém manter se possível.
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<EncomendaDTO>> Checkout([FromBody] CheckoutDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Estado != "Ativo") return StatusCode(403, "Conta inativa.");

            try
            {
                var novaEncomenda = await _encomendaRepository.CriarEncomenda(userId, dto);

                // Mapeamento DTO
                var resultado = new EncomendaDTO
                {
                    Id = novaEncomenda.Id,
                    Data = novaEncomenda.Data,
                    Estado = novaEncomenda.Estado,
                    ValorTotal = novaEncomenda.ValorTotal,
                    MetodoPagamento = novaEncomenda.MetodoPagamento ?? "",
                    MoradaEnvio = novaEncomenda.MoradaEnvio ?? "",
                    MetodoEntrega = novaEncomenda.MetodoEntrega ?? "",
                    Itens = novaEncomenda.Itens.Select(i => new ItemCarrinhoDTO
                    {
                        ProdutoId = i.ProdutoId,
                        Nome = i.Produto?.Nome ?? "Produto",
                        Quantidade = i.Quantidade,
                        Preco = i.PrecoUnitario
                    }).ToList()
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // 2. LISTAR ENCOMENDAS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EncomendaDTO>>> GetEncomendas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<Encomenda> encomendas;

            if (User.IsInRole("Administrador") || User.IsInRole("Funcionario"))
            {
                encomendas = await _encomendaRepository.GetAllEncomendas();
            }
            else
            {
                encomendas = await _encomendaRepository.GetEncomendasDoCliente(userId!);
            }

            var dtos = encomendas.Select(e => new EncomendaDTO
            {
                Id = e.Id,
                Data = e.Data,
                Estado = e.Estado,
                ValorTotal = e.ValorTotal,
                MetodoPagamento = e.MetodoPagamento ?? "",
                MoradaEnvio = e.MoradaEnvio ?? "",
                MetodoEntrega = e.MetodoEntrega ?? "",
                Itens = e.Itens.Select(i => new ItemCarrinhoDTO
                {
                    ProdutoId = i.ProdutoId,
                    Nome = i.Produto?.Nome ?? "Produto",
                    Quantidade = i.Quantidade,
                    Preco = i.PrecoUnitario
                }).ToList()
            });

            return Ok(dtos);
        }

        // 3. DETALHES
        [HttpGet("{id}")]
        public async Task<ActionResult<EncomendaDTO>> GetDetalhes(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Encomenda? encomenda;

            if (User.IsInRole("Administrador") || User.IsInRole("Funcionario"))
            {
                encomenda = await _encomendaRepository.GetEncomendaPorId(id);
            }
            else
            {
                encomenda = await _encomendaRepository.GetDetalhesEncomenda(userId!, id);
            }

            if (encomenda == null) return NotFound(new { Message = "Encomenda não encontrada." });

            var dto = new EncomendaDTO
            {
                Id = encomenda.Id,
                Data = encomenda.Data,
                Estado = encomenda.Estado,
                ValorTotal = encomenda.ValorTotal,
                MetodoPagamento = encomenda.MetodoPagamento ?? "",
                MoradaEnvio = encomenda.MoradaEnvio ?? "",
                MetodoEntrega = encomenda.MetodoEntrega ?? "",
                Itens = encomenda.Itens.Select(i => new ItemCarrinhoDTO
                {
                    ProdutoId = i.ProdutoId,
                    Nome = i.Produto?.Nome ?? "Produto",
                    Quantidade = i.Quantidade,
                    Preco = i.PrecoUnitario
                }).ToList()
            };

            return Ok(dto);
        }

        // 4. CONFIRMAR RECEÇÃO (CORRIGIDO)
        [HttpPatch("{id}/confirmar-entrega")]
        public async Task<IActionResult> ConfirmarEntrega(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Validar se a encomenda existe e pertence ao cliente
            var encomenda = await _encomendaRepository.GetDetalhesEncomenda(userId, id);

            if (encomenda == null)
                return NotFound(new { Message = "Encomenda não encontrada." });

            if (!encomenda.Estado.Equals("Enviado", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { Message = $"Não é possível confirmar. Estado atual: {encomenda.Estado}" });

            try
            {
                await _encomendaRepository.AtualizarEstado(id, "Entregue");
                return Ok(new { Message = "Encomenda marcada como entregue." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}