using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "A morada de envio é obrigatória")]
        public string MoradaEnvio { get; set; } = string.Empty;

        [Required(ErrorMessage = "O método de pagamento é obrigatório")]
        public string MetodoPagamento { get; set; } = string.Empty; // Ex: "MBWay", "Visa"

        [Required(ErrorMessage = "O método de entrega é obrigatório")]
        public string MetodoEntrega { get; set; } = string.Empty; // Ex: "CTT Expresso"
    }
}