using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestaoLoja.Data;

namespace GestaoLoja.Entities
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        // --- NOVO: Imagem na Base de Dados ---
        public byte[]? Imagem { get; set; }
        public string? ImagemUrl { get; set; } // Mantém para guardar o nome do ficheiro ou tipo MIME (ex: "image/png")

        // --- NOVO: Distinção Listagem vs Venda ---
        [Display(Name = "Disponível para Venda")]
        public bool ParaVenda { get; set; } = true; // Se false, é apenas para "Listagem" (Coleção)

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