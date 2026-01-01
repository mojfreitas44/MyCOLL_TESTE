using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class CheckoutDto
    {
        [Required]
        public string MoradaEnvio { get; set; } = string.Empty;

        [Required]
        public string MetodoPagamento { get; set; } = string.Empty;

        [Required]
        public int ModoEntregaId { get; set; }  // Ex: 2 (CTT)

        public List<ItemCarrinhoDTO> Itens { get; set; } = new();


        // Campos de Cartão (Simulação)
        public string? NumeroCartao { get; set; }
        public string? Validade { get; set; }
        public string? CVV { get; set; }
    }
}