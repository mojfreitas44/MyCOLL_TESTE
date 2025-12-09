namespace API.DTO
{
    public class EncomendaDTO
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorTotal { get; set; }
        public string Estado { get; set; } = string.Empty;

        // Dados de Envio
        public string MoradaEnvio { get; set; } = string.Empty;
        public string MetodoPagamento { get; set; } = string.Empty;
        public string MetodoEntrega { get; set; } = string.Empty;

        // Lista de produtos comprados
        public List<ItemEncomendaDTO> Itens { get; set; } = new List<ItemEncomendaDTO>();
    }

    public class ItemEncomendaDTO
    {
        public string ProdutoNome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}