namespace API.DTO
{
    public class ItemCarrinhoDTO
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal Preco { get; set; } // Será o teu PrecoVenda
        public string? ImagemUrl { get; set; } // Opcional, se quiseres mostrar
    }
}