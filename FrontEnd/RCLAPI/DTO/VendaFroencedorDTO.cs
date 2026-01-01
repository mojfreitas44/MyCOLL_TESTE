namespace RCLAPI.DTO
{
    public class VendaFornecedorDTO
    {
        public int EncomendaId { get; set; }
        public DateTime DataVenda { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal TotalGanho { get; set; }
        public string EstadoEncomenda { get; set; } = string.Empty;
    }
}