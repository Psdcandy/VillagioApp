
public class LoginRequest
{
    public int TipoUsuarioId { get; set; }
    public string? Nome { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? CNPJ { get; set; }
    public string Senha { get; set; } = "";
}