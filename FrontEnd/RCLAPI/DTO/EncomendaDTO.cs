using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCLAPI.DTO
{
    public class EncomendaItemDTO
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty; // O nome do produto vem aqui
        public int Quantidade { get; set; }
        public decimal Preco { get; set; } // Preço unitário
    }
    public class EncomendaDTO
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public string MetodoPagamento { get; set; } = string.Empty;
        public string MoradaEnvio { get; set; } = string.Empty;
        public string MetodoEntrega { get; set; } = string.Empty;

        // Lista de produtos dentro da encomenda
        public List<EncomendaItemDTO> Itens { get; set; } = new();
    }
}
