namespace VillagioApi.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = ""; // Obrigatório para ambos
        public string Telefone { get; set; } = ""; // Obrigatório para ambos
        public string? Email { get; set; } // Não usado mais para login, mas mantido para compatibilidade
        public string? CNPJ { get; set; } // Obrigatório para Agência
        public string Senha { get; set; } = ""; // Sempre obrigatório
        public int TipoUsuarioId { get; set; } // 1 = Família, 2 = Agência

        public TipoUsuario? TipoUsuario { get; set; }
    }
}
