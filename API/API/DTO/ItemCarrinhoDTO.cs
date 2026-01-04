namespace API.DTO
{
    public class ItemCarrinhoDTO
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal Preco { get; set; } // Preço unitário do produto
        public string? ImagemUrl { get; set; } // URL da imagem do produto
    }
}