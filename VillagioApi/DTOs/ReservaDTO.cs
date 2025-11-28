public class ReservaDTO
{
    public DateTime Data { get; set; }
    public string HorarioTexto { get; set; } = string.Empty;
    public int TipoUsuarioId { get; set; }
    public int VagasReservadas { get; set; }
}
