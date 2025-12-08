using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class ModoEntrega
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Preco { get; set; }
    }
}