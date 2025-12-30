namespace API.DTO
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal PrecoVenda { get; set; }
        public byte[]? Imagem { get; set; }
        public string Condicao { get; set; } = string.Empty;
        public bool ParaVenda { get; set; } // Venda ou Coleção
        public string Estado { get; set; } = ""; // Ativos ou Pendentes
        public int Stock { get; set; } // Quantidade em Stock
        public string? CategoriaNome { get; set; }
        public int CategoriaId { get; set; }
        public string? FornecedorNome { get; set; }
        public string? Disponibilidade { get; set; } = string.Empty;
    }
}