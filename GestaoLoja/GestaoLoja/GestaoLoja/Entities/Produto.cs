using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestaoLoja.Data;

namespace GestaoLoja.Entities
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        // --- Imagem na BD ---
        public byte[]? Imagem { get; set; }
        public string? ImagemUrl { get; set; } // Opcional: URL para a imagem armazenada externamente

        // --- Listagem vs Venda ---
        [Display(Name = "Disponível para Venda?")]
        public bool ParaVenda { get; set; } = true; // Se false, aparece apenas como "Coleção"

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecoBase { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecoVenda { get; set; }

        public int Stock { get; set; }
        public string Condicao { get; set; } = "Usado";
        public string Estado { get; set; } = "Pendente";

        // Chaves Estrangeiras
        public int CategoriaId { get; set; }
        public virtual Categoria? Categoria { get; set; }

        public string? FornecedorId { get; set; }
        [ForeignKey("FornecedorId")]
        public virtual ApplicationUser? Fornecedor { get; set; }
    }
}