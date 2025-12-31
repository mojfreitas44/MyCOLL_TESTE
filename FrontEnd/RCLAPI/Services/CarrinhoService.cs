using Blazored.LocalStorage;
using RCLAPI.DTO;

namespace RCLAPI.Services
{
    public class CarrinhoService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly string _key = "carrinho"; // Chave para guardar no browser

        // Evento para avisar os componentes (ex: atualizar o número no menu)
        public event Action? OnChange;

        public CarrinhoService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<List<ItemCarrinho>> GetItens()
        {
            var carrinho = await _localStorage.GetItemAsync<List<ItemCarrinho>>(_key);
            return carrinho ?? new List<ItemCarrinho>();
        }

        public async Task AdicionarAoCarrinho(ProdutoDTO produto)
        {
            var carrinho = await GetItens();
            var itemExistente = carrinho.FirstOrDefault(i => i.Produto.Id == produto.Id);

            if (itemExistente == null)
            {
                carrinho.Add(new ItemCarrinho { Produto = produto, Quantidade = 1 });
            }
            else
            {
                // Se já existe, aumenta a quantidade, mas valida o stock maximo
                if (itemExistente.Quantidade < produto.Stock)
                {
                    itemExistente.Quantidade++;
                }
            }

            await _localStorage.SetItemAsync(_key, carrinho);
            OnChange?.Invoke(); // Notifica a app que houve mudanças
        }

        public async Task RemoverDoCarrinho(int produtoId)
        {
            var carrinho = await GetItens();
            var item = carrinho.FirstOrDefault(i => i.Produto.Id == produtoId);

            if (item != null)
            {
                carrinho.Remove(item);
                await _localStorage.SetItemAsync(_key, carrinho);
                OnChange?.Invoke();
            }
        }

        public async Task IncrementarQuantidade(int produtoId)
        {
            var carrinho = await GetItens();
            var item = carrinho.FirstOrDefault(i => i.Produto.Id == produtoId);

            if (item != null && item.Quantidade < item.Produto.Stock)
            {
                item.Quantidade++;
                await _localStorage.SetItemAsync(_key, carrinho);
                OnChange?.Invoke();
            }
        }

        public async Task DecrementarQuantidade(int produtoId)
        {
            var carrinho = await GetItens();
            var item = carrinho.FirstOrDefault(i => i.Produto.Id == produtoId);

            if (item != null)
            {
                if (item.Quantidade > 1)
                {
                    item.Quantidade--;
                    await _localStorage.SetItemAsync(_key, carrinho);
                }
                else
                {
                    // Se for 1 e diminuir, remove o item
                    carrinho.Remove(item);
                    await _localStorage.SetItemAsync(_key, carrinho);
                }
                OnChange?.Invoke();
            }
        }

        public async Task LimparCarrinho()
        {
            await _localStorage.RemoveItemAsync(_key);
            OnChange?.Invoke();
        }

        public async Task<int> GetContagemTotal()
        {
            var carrinho = await GetItens();
            return carrinho.Sum(i => i.Quantidade);
        }

        public async Task<decimal> GetPrecoTotal()
        {
            var carrinho = await GetItens();
            return carrinho.Sum(i => i.PrecoTotal);
        }
    }

    // Classe auxiliar simples para usar no carrinho
    public class ItemCarrinho
    {
        public ProdutoDTO Produto { get; set; } = new();
        public int Quantidade { get; set; }
        public decimal PrecoTotal => Produto.PrecoVenda * Quantidade;
    }
}