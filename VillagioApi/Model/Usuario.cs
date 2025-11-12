namespace VillagioApi.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Telefone { get; set; }
        public string? Email { get; set; }
        public string? CNPJ { get; set; }
        public required string Senha { get; set; }
        public int TipoUsuarioId { get; set; }
        public TipoUsuario? TipoUsuario { get; set; }
    }

}
