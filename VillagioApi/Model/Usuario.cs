using VillagioApi.Model;

public class Usuario
{
    public int Id { get; set; }
    public string? Nome { get; set; }       // Agora opcional
    public string? Telefone { get; set; }   // Agora opcional
    public string? Email { get; set; }
    public string? CNPJ { get; set; }
    public string Senha { get; set; } = ""; // Pode manter obrigatório com valor padrão
    public int TipoUsuarioId { get; set; }
    public TipoUsuario? TipoUsuario { get; set; }
}
