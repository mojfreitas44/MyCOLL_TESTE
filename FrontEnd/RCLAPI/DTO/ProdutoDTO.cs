using System.Text.Json.Serialization;

namespace RCLAPI.DTO
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // --- CORREÇÃO: O nome agora é IGUAL à API ---
        public decimal PrecoVenda { get; set; }

        public bool ParaVenda { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int CategoriaId { get; set; } //Testando
        public string? CategoriaNome { get; set; }
        public string? FornecedorNome { get; set; }
        public string Condicao { get; set; } = string.Empty;
        public string? Disponibilidade { get; set; }
        public byte[]? Imagem { get; set; }

        // Lógica de Visualização
        public string ImagemSrc
        {
            get
            {
                if (Imagem == null || Imagem.Length == 0) return string.Empty;
                return $"data:image/png;base64,{Convert.ToBase64String(Imagem)}";
            }
        }

        public string DisponibilidadeTexto => !string.IsNullOrEmpty(Disponibilidade) ? Disponibilidade : (Stock <= 0 ? "Esgotado" : "Em Stock");
        public string DisponibilidadeCor => Stock <= 0 ? "text-danger" : Stock <= 5 ? "text-warning" : "text-success";
    }
}