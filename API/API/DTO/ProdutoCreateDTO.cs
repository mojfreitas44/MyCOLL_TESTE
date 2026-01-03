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
        
        [Range(1, int.MaxValue, ErrorMessage = "O stock deve ser pelo menos 1")]
        public int Stock { get; set; }
        
        [Required]
        public string Condicao { get; set; } = "Usado"; // Novo ou Usado

        [Range(0, int.MaxValue, ErrorMessage = "A categoria é obrigatória")]
        public int CategoriaId { get; set; }
        public string? ImagemBase64 { get; set; }
        public bool RemoverImagem { get; set; } = false;
        public bool ParaVenda { get; set; } = true;
    }
}