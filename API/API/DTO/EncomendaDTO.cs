namespace API.DTO
{
    public class EncomendaDTO
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorTotal { get; set; }
        public string Estado { get; set; } = string.Empty;

        public string MoradaEnvio { get; set; } = string.Empty;
        public string MetodoPagamento { get; set; } = string.Empty;
        public string MetodoEntrega { get; set; } = string.Empty;

        public List<ItemCarrinhoDTO> Itens { get; set; } = new();
    }
}