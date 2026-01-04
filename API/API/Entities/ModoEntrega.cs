using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities 
{
    public class ModoEntrega
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Preco { get; set; } = 0;
    }
}