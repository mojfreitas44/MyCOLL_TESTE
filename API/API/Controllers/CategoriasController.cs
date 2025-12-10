using API.DTO;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public CategoriasController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        // GET /api/Categorias
        // GET /api/Categorias?parentId=5
        // GET /api/Categorias?tipo=Moedas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get(
            [FromQuery] int? parentId,
            [FromQuery] string? tipo)
        {
            // 1. Contagens (Apenas produtos ParaVenda)
            var counts = await _ctx.Produtos
                .Where(p => p.ParaVenda == true && p.Estado == "Ativo")
                .GroupBy(p => p.CategoriaId)
                .Select(g => new { g.Key, Total = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Total);

            // 2. Query Base
            IQueryable<Categoria> q = _ctx.Categorias.AsNoTracking();

            // 3. Lógica do "Tipo": Traduz texto (ex: "moedas") para o ID da Categoria Pai
            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var norm = Normalize(tipo);

                // Vai buscar todas as raízes (Categorias sem pai)
                var roots = await _ctx.Categorias
                    .AsNoTracking()
                    .Where(c => c.CategoriaPaiId == null)
                    .ToListAsync();

                // Encontra a primeira raiz que combine com o nome
                var foundRoot = roots.FirstOrDefault(c =>
                    Normalize(c.Nome).Contains(norm) || norm.Contains(Normalize(c.Nome)));

                // Se encontrou (ex: ID da Moeda é 1), define o parentId = 1
                if (foundRoot != null)
                {
                    parentId = foundRoot.Id;
                }
            }

            // 4. Filtro por Pai
            // Se o parentId foi definido (diretamente ou via 'tipo'), filtramos pelos FILHOS
            if (parentId.HasValue)
            {
                q = q.Where(c => c.CategoriaPaiId == parentId.Value);
            }

            // 5. Ordenação e Projeção
            var data = await q
                .OrderBy(c => c.Nome)
                .Select(c => new CategoriaDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    CategoriaPaiId = c.CategoriaPaiId,
                    TotalProdutos = counts.ContainsKey(c.Id) ? counts[c.Id] : 0
                })
                .ToListAsync();

            return Ok(data);
        }

        // Função auxiliar obrigatória para o 'tipo' funcionar
        static string Normalize(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var formD = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in formD)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString()
                     .Normalize(NormalizationForm.FormC)
                     .ToLowerInvariant()
                     .Trim();
        }
    }
}