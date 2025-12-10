using API.Entities;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModoEntregaController : ControllerBase
    {
        private readonly IModoEntregaRepository _repo;

        public ModoEntregaController(IModoEntregaRepository repo)
        {
            _repo = repo;
        }

        // GET: api/ModosEntrega
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModoEntrega>>> Get()
        {
            var modos = await _repo.GetAllAsync();
            return Ok(modos);
        }

        // GET: api/ModosEntrega/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModoEntrega>> Get(int id)
        {
            var modo = await _repo.GetByIdAsync(id);
            if (modo == null) return NotFound();
            return Ok(modo);
        }
    }
}