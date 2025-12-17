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
    [Authorize] // Autenticação básica obrigatória para todos
    public class EncomendasController : ControllerBase
    {
        private readonly IEncomendaRepository _encomendaRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EncomendasController(IEncomendaRepository encomendaRepository, UserManager<ApplicationUser> userManager)
        {
            _encomendaRepository = encomendaRepository;
            _userManager = userManager;
        }

        // 1. CRIAR ENCOMENDA (Checkout)
        // LÓGICA: Apenas 'Cliente' pode criar encomendas.
        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<EncomendaDTO>> Checkout([FromBody] CheckoutDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Verificação extra de segurança da conta
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Estado != "Ativo") return StatusCode(403, "Conta inativa.");

            try
            {
                var novaEncomenda = await _encomendaRepository.CriarEncomenda(userId, dto);

                // Mapeamento manual para DTO
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
        // LÓGICA: Admin/Funcionario vê TUDO. Cliente vê SÓ AS SUAS.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EncomendaDTO>>> GetEncomendas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<Encomenda> encomendas;

            // Se for Gestão, vê o histórico global
            if (User.IsInRole("Administrador") || User.IsInRole("Funcionario"))
            {
                encomendas = await _encomendaRepository.GetAllEncomendas();
            }
            else
            {
                // Se for Cliente, vê apenas as suas
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

        // 3. DETALHES DA ENCOMENDA
        // LÓGICA: Admin vê qualquer uma. Cliente só vê se for dele.
        [HttpGet("{id}")]
        public async Task<ActionResult<EncomendaDTO>> GetDetalhes(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Encomenda? encomenda;

            if (User.IsInRole("Administrador") || User.IsInRole("Funcionario"))
            {
                // Admin procura sem filtro de utilizador
                encomenda = await _encomendaRepository.GetEncomendaPorId(id);
            }
            else
            {
                // Cliente procura com filtro de utilizador (Segurança)
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
    }
}