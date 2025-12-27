using System.Text.Json.Serialization;

namespace RCLAPI.DTO
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Detalhe { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string ImagemUrl { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
    }

    public class ProdutoListDTO : ProdutoDTO { }

    public class ProdutoDetaislDTO : ProdutoDTO
    {
        public bool EmStock { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
    }

    public class ProdutoFavorito : ProdutoDTO { }

    public class ProdutoFornecedorDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Detalhe { get; set; }
        public decimal PrecoBase { get; set; }
        public bool ParaVenda { get; set; }
        public bool ParaAluguer { get; set; }
        public int CategoriaId { get; set; }
        public int TipoProduto { get; set; } // 0 ou 1
        public int EmStock { get; set; }
        public int? ModoEntregaId { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int? Ano { get; set; }
        public string? TipoCombustivel { get; set; }
        public string? Imagem { get; set; }
    }

    public class ListaVendasItemDTO
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public int QuantidadeVendida { get; set; }
        public decimal TotalFaturado { get; set; }
    }
}