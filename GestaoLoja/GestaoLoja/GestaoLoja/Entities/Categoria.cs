using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoLoja.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descricao { get; set; }

        // --- NOVO: Suporte para Hierarquia ---
        public int? CategoriaPaiId { get; set; }

        [ForeignKey("CategoriaPaiId")]
        public virtual Categoria? CategoriaPai { get; set; }

        // Subcategorias
        public virtual ICollection<Categoria> SubCategorias { get; set; } = new List<Categoria>();
        // --------------------------------------------------------------

        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}