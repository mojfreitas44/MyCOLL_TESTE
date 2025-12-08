namespace API.DTO
{
    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int TotalProdutos { get; set; }
        public int? CategoriaPaiId { get; set; }
    }
}