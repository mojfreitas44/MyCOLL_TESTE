using System.Text.Json.Serialization;

namespace RCLAPI.DTO
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // IMPORTANTE: Tem de ter o mesmo nome que vem da API
        [JsonPropertyName("precoVenda")]
        public decimal PrecoVenda { get; set; }

        public byte[]? Imagem { get; set; }
        public string Condicao { get; set; } = string.Empty;

        public string? CategoriaNome { get; set; }
        public int CategoriaId { get; set; }
        public string? FornecedorNome { get; set; }
        public string? Disponibilidade { get; set; } = string.Empty;
    }
}