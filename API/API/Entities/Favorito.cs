using API.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Favorito
    {
        public int Id { get; set; }

        [ForeignKey("Cliente")]
        public string ClienteId { get; set; } = string.Empty;
        public ApplicationUser? Cliente { get; set; }

        [ForeignKey("Produto")]
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
    }
}