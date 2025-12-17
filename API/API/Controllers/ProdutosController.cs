using Microsoft.AspNetCore.Mvc;
using API.DTO;
using API.Repositories;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutosController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        // GET: api/Produtos
        // Serve para a Página Inicial, Pesquisa e Filtros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] string? pesquisa, [FromQuery] int? categoriaId)
        {
            // CORREÇÃO: Usamos o GetAllAsync que está na tua Interface
            var produtos = await _produtoRepository.GetAllAsync(pesquisa, categoriaId);

            var produtosDto = produtos.Select(p => new ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                PrecoVenda = p.PrecoVenda,
                Imagem = p.Imagem,
                Condicao = p.Condicao,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.Categoria?.Nome,
                // A correção do Fornecedor para evitar "vazio"
                FornecedorNome = !string.IsNullOrEmpty(p.Fornecedor?.Nome)
                    ? p.Fornecedor.Nome : (p.Fornecedor?.UserName ?? "Produto Oficial"),
                Disponibilidade = p.Estado
            }).ToList();

            return Ok(produtosDto);
        }

        // GET: api/Produtos/destaque
        
        [HttpGet("destaque")]
        public async Task<ActionResult<ProdutoDTO>> GetDestaque()
        {
            var produto = await _produtoRepository.GetProdutoDestaqueAsync();

            if (produto == null)
            {
                return NotFound("Nenhum produto em destaque encontrado.");
            }

            var dto = new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                PrecoVenda = produto.PrecoVenda,
                Imagem = produto.Imagem,
                Condicao = produto.Condicao,
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome,
                FornecedorNome = !string.IsNullOrEmpty(produto.Fornecedor?.Nome)
                    ? produto.Fornecedor.Nome : (produto.Fornecedor?.UserName ?? "Produto Oficial"),
                Disponibilidade = produto.Estado
            };

            return Ok(dto);
        }

        // GET: api/Produtos/5
        // Serve para a página de detalhes do produto
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            var dto = new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                PrecoVenda = produto.PrecoVenda,
                Imagem = produto.Imagem,
                Condicao = produto.Condicao,
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome,
                FornecedorNome = !string.IsNullOrEmpty(produto.Fornecedor?.Nome)
                   ? produto.Fornecedor.Nome : (produto.Fornecedor?.UserName ?? "Produto Oficial"),
                Disponibilidade = produto.Estado
            };

            return Ok(dto);
        }
    }
}