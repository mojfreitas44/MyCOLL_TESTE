namespace API.DTO
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Nome correto igual à Base de Dados
        public decimal PrecoVenda { get; set; }

        public byte[]? Imagem { get; set; }
        public string Condicao { get; set; } = string.Empty;

        // O "?" permite que seja nulo, evitando avisos amarelos
        public string? CategoriaNome { get; set; }
        public int CategoriaId { get; set; }
        public string? FornecedorNome { get; set; }
    }
}