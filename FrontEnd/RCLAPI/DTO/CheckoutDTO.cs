using System.ComponentModel.DataAnnotations;

namespace RCLAPI.DTO
{
    public class CheckoutDTO
    {
        public int UtilizadorId { get; set; }

        public List<ItemCarrinhoDTO> Itens { get; set; } = new();

        [Required(ErrorMessage = "A morada é obrigatória")]
        public string MoradaEnvio { get; set; } = string.Empty;

        [Required]
        public int ModoEntregaId { get; set; }

        [Required(ErrorMessage = "Escolha Visa ou Mastercard")]
        public string MetodoPagamento { get; set; } = string.Empty;

        // --- VALIDAÇÕES DE PAGAMENTO ---

        [RegularExpression(@"^\d{16}$", ErrorMessage = "O cartão tem de ter 16 dígitos numéricos.")]
        public string? NumeroCartao { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Use o formato MM/AA (ex: 05/26).")]
        public string? Validade { get; set; }

        [RegularExpression(@"^\d{3}$", ErrorMessage = "O CVV tem de ter 3 dígitos numéricos.")]
        public string? CVV { get; set; }
    }

    public class ItemCarrinhoDTO
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}