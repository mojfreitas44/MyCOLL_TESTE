using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class ProdutoCreateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Descricao { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que 0")]
        public decimal PrecoVenda { get; set; }

        [Required]
        public string Condicao { get; set; } = "Usado"; // Novo ou Usado

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int CategoriaId { get; set; }

        // A imagem vem como texto (Base64) da App e nós convertemos para bytes
        public string? ImagemBase64 { get; set; }
    }
}