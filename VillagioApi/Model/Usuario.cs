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
        public byte Tipo { get; set; } // 0 = ADM, 1 = Família, 2 = Agência
    }
}
