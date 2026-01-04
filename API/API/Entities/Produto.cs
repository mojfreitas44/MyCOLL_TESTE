using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Data;

namespace API.Entities
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecoBase { get; set; } // Preço de Custo (Obrigatório na BD)

        public int Stock { get; set; } // Quantidade (Obrigatório na BD)
        // ---------------------------

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecoVenda { get; set; } // Preço ao Público

        public byte[]? Imagem { get; set; }
        public bool ParaVenda { get; set; } = true;
        public string Condicao { get; set; } = "Usado";
        public string Estado { get; set; } = "Pendente";

        // Estrangeiras
        public int CategoriaId { get; set; }
        public virtual Categoria? Categoria { get; set; }

        public string? FornecedorId { get; set; }
        [ForeignKey("FornecedorId")]
        public virtual ApplicationUser? Fornecedor { get; set; }
    }
}