using System.Text.Json.Serialization;

namespace RCLAPI.DTO
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("precoVenda")]
        public decimal Preco { get; set; }

        // --- ATUALIZAÇÃO: ADICIONADO PARAVENDA E MAPEAMENTOS ---

        [JsonPropertyName("stock")]
        public int Stock { get; set; }

        [JsonPropertyName("paraVenda")]
        public bool ParaVenda { get; set; } // <--- O CAMPO CRÍTICO QUE FALTAVA

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        // -------------------------------------------------------

        // Lógica de Disponibilidade (MANTIDA)
        public string DisponibilidadeTexto
        {
            get
            {
                if (Stock <= 0) return "Esgotado";
                if (Stock < 5) return "Últimas Unidades!";
                return "Em Stock";
            }
        }

        public string DisponibilidadeCor
        {
            get
            {
                if (Stock <= 0) return "text-danger";
                if (Stock < 5) return "text-warning";
                return "text-success";
            }
        }

        public string? CategoriaNome { get; set; }
        public string? FornecedorNome { get; set; }
        public string Condicao { get; set; } = string.Empty;

        public byte[]? Imagem { get; set; }

        // Conversão de Imagem (MANTIDA)
        public string ImagemSrc
        {
            get
            {
                if (Imagem == null || Imagem.Length == 0)
                    return string.Empty;

                string base64 = Convert.ToBase64String(Imagem);
                return $"data:image/png;base64,{base64}";
            }
        }
    }
}