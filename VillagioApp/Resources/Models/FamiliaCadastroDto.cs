namespace VillagioApp.Resources.Models
{
    public class FamiliaCadastroDto
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Senha { get; set; }
        public int TipoUsuarioId { get; set; } = 1; // Família
    }
}
