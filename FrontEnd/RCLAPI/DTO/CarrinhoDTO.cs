namespace RCLAPI.DTO
{
    public class ItemCarrinhoCompra
    {
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public string ImagemUrl { get; set; }
    }
}