using System.ComponentModel.DataAnnotations.Schema;
using API.Data; // <--- Importante para encontrar o ApplicationUser

namespace API.Entities
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        public byte[]? Imagem { get; set; }
        public string? ImagemUrl { get; set; }

        public bool ParaVenda { get; set; } = true;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecoBase { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecoVenda { get; set; }

        public int Stock { get; set; }
        public string Condicao { get; set; } = "Usado";
        public string Estado { get; set; } = "Pendente";

        public int CategoriaId { get; set; }
        public virtual Categoria? Categoria { get; set; }

        public string? FornecedorId { get; set; }
        [ForeignKey("FornecedorId")]
        public virtual ApplicationUser? Fornecedor { get; set; }
    }
}