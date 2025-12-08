using System.ComponentModel.DataAnnotations.Schema;
using API.Data;

namespace API.Entities
{
    public class Encomenda
    {
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValorTotal { get; set; }

        public string Estado { get; set; } = "Pendente";

        public string? MoradaEnvio { get; set; }
        public string? MetodoPagamento { get; set; }
        public string? MetodoEntrega { get; set; }

        public string? ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public virtual ApplicationUser? Cliente { get; set; }

        public virtual ICollection<EncomendaItem> Itens { get; set; } = new List<EncomendaItem>();
    }

    public class EncomendaItem
    {
        public int Id { get; set; }
        public int EncomendaId { get; set; }
        public virtual Encomenda Encomenda { get; set; } = null!;

        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; } = null!;

        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecoUnitario { get; set; }
    }
}