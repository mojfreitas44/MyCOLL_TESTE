using API.DTO;
using GestaoLoja.Data;
using GestaoLoja.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Produtos
        // Exemplos de uso:
        //  - Tudo: api/Produtos
        //  - Pesquisa: api/Produtos?search=Dinis
        //  - Por Categoria: api/Produtos?categoriaId=5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutos(
            [FromQuery] string? search,
            [FromQuery] int? categoriaId)
        {
            // 1. Query Base: Só queremos produtos marcados "ParaVenda"
            var query = _context.Produtos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .Where(p => p.ParaVenda == true);

            // 2. Filtro de Texto (Nome ou Descrição)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Nome.Contains(search) || p.Descricao.Contains(search));
            }

            // 3. Filtro de Categoria
            if (categoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaId == categoriaId);
            }

            // 4. Transformar em DTO
            var produtos = await query
                .Select(p => new ProdutoDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.PrecoVenda, // Mapeamos o teu PrecoVenda
                    Imagem = p.Imagem,
                    Condicao = p.Condicao,
                    CategoriaId = p.CategoriaId,
                    CategoriaNome = p.Categoria != null ? p.Categoria.Nome : "Sem Categoria",
                    FornecedorNome = p.Fornecedor != null ? p.Fornecedor.UserName : "Loja"
                })
                .ToListAsync();

            return Ok(produtos);
        }

        // GET: api/Produtos/5
        // Para ver os detalhes de um produto específico
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDTO>> GetProduto(int id)
        {
            var p = await _context.Produtos
                .AsNoTracking()
                .Include(c => c.Categoria)
                .Include(f => f.Fornecedor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
            {
                return NotFound();
            }

            var dto = new ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.PrecoVenda,
                Imagem = p.Imagem,
                Condicao = p.Condicao,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.Categoria?.Nome ?? "Sem Categoria",
                FornecedorNome = p.Fornecedor?.UserName
            };

            return Ok(dto);
        }
    }
}