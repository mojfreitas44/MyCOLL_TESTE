using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoLoja.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        // --- Hierarquia para as categorias ---
        // Permite: Moedas (Pai) -> Portugal (Filho) -> D.Dinis (Neto)
        [Display(Name = "Categoria Pai")]
        public int? CategoriaPaiId { get; set; }

        [ForeignKey("CategoriaPaiId")]
        public virtual Categoria? CategoriaPai { get; set; }

        public virtual ICollection<Categoria> SubCategorias { get; set; } = new List<Categoria>();
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}