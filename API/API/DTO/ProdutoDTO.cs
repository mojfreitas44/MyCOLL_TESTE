namespace API.DTO
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }      // O teu "PrecoVenda"
        public byte[]? Imagem { get; set; }
        public string Condicao { get; set; } = string.Empty; // "Novo" ou "Usado"
        public string CategoriaNome { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string? FornecedorNome { get; set; } // Para saber quem vende (opcional)
    }
}