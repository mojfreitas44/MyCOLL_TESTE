using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Data; // <--- MUDANÇA CRÍTICA: Aponta para o ApplicationUser LOCAL da API

namespace API.Entities // <--- MUDANÇA IMPORTANTE
{
    public class Encomenda
    {
        public int Id { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValorTotal { get; set; }

        public string Estado { get; set; } = "Pendente";

        // Dados de Envio
        public string? MoradaEnvio { get; set; }
        public string? MetodoPagamento { get; set; }
        public string? MetodoEntrega { get; set; }

        // Relações
        public string? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        // AQUI USA O "API.Data.ApplicationUser" AUTOMATICAMENTE
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