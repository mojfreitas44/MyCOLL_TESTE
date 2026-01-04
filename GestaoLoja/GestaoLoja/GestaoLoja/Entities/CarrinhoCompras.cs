using GestaoLoja.Data; 
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoLoja.Entities 
{
    public class CarrinhoCompras
    {
        public int Id { get; set; }

        public string ClienteId { get; set; } = string.Empty;

        [ForeignKey("ClienteId")]
        public virtual ApplicationUser? Cliente { get; set; }

        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual Produto? Produto { get; set; }

        public int Quantidade { get; set; }
    }
}