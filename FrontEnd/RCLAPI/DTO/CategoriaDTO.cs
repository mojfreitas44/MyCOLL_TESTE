namespace RCLAPI.DTO
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    // Herdamos de Categoria para manter a estrutura
    public class CategoriaDTO : Categoria
    {
    }
}