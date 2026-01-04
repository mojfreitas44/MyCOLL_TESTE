namespace RCLAPI.DTO
{
    public class EditarPerfilModel
    {
        public string Nome { get; set; } = "";
        public string Apelido { get; set; } = "";
        public long NIF { get; set; }
        public string Telemovel { get; set; } = "";
        public MoradaDTO Morada { get; set; } = new MoradaDTO();
    }

    public class MoradaDTO
    {
        public string Rua { get; set; } = "";
        public string Localidade { get; set; } = "";
        public string CodigoPostal { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Pais { get; set; } = "";
    }

    // Classe auxiliar para receber os dados do GET 
    public class UserPerfilResponse
    {
        public string Id { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Apelido { get; set; } = "";
        public long NIF { get; set; }
        public string Telemovel { get; set; } = "";

        // Morada 
        public string Rua { get; set; } = "";
        public string Localidade { get; set; } = "";
        public string CodigoPostal { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Pais { get; set; } = "";
    }
}