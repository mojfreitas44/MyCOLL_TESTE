namespace RCLAPI.DTO
{
    public class EncomendaDTO
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public List<EncomendaItemDTO> Itens { get; set; } = new();
    }

    public class EncomendaItemDTO
    {
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }

    public class CheckoutDTO
    {
        public int ModoEntregaId { get; set; }
        public string MoradaEntrega { get; set; }
        public string Observacoes { get; set; }
    }

    public class PagamentoDTO
    {
        public string NumeroCartao { get; set; }
        public string Validade { get; set; }
        public string CVV { get; set; }
    }
}