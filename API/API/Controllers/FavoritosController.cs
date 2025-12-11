using API.DTO;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cliente")]
    public class FavoritosController : ControllerBase
    {
        private readonly IFavoritoRepository _repo;

        public FavoritosController(IFavoritoRepository repo)
        {
            _repo = repo;
        }

        // GET: api/Favoritos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetMeusFavoritos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favoritos = await _repo.GetFavoritosDoCliente(userId!);
            return Ok(favoritos);
        }

        // POST: api/Favoritos/5 (Adicionar o produto 5 aos favoritos)
        [HttpPost("{produtoId}")]
        public async Task<IActionResult> Adicionar(int produtoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _repo.AdicionarFavorito(userId!, produtoId);
            return Ok(new { Message = "Adicionado aos favoritos" });
        }

        // DELETE: api/Favoritos/5 (Remover o produto 5)
        [HttpDelete("{produtoId}")]
        public async Task<IActionResult> Remover(int produtoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _repo.RemoverFavorito(userId!, produtoId);
            return NoContent();
        }
    }
}