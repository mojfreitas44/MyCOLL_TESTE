using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Data; // <--- Importante: Aponta para o User local

namespace API.Entities // <--- Namespace da API
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal PrecoVenda { get; set; }
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