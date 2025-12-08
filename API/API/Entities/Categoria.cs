using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }

        public int? CategoriaPaiId { get; set; }
        [ForeignKey("CategoriaPaiId")]
        public virtual Categoria? CategoriaPai { get; set; }

        public virtual ICollection<Categoria> SubCategorias { get; set; } = new List<Categoria>();
        // Nota: Na API, muitas vezes evitamos ICollection<Produto> aqui para não criar ciclos infinitos no JSON,
        // mas podes manter para já.
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}