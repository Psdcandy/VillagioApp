using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly SqlConnection _connection;

    public ReservasController(IConfiguration config)
    {
        _connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    }

    /// <summary>
    /// Retorna horários disponíveis para uma data, considerando quantidade de visitantes e tipo de usuário.
    /// </summary>
    [HttpGet("disponibilidade")]
    public async Task<IActionResult> GetDisponibilidade(DateTime data, int quantidade, int tipoUsuarioId)
    {
        const int VagasPorHorario = 50;

        var horarios = new List<string>
        {
            "08:00", "09:00", "10:00", "11:00",
            "12:00", "14:00", "15:00",
            "16:00", "17:00"
        };

        var resultado = new List<object>();

        foreach (var horario in horarios)
        {
            string query = @"
                SELECT ISNULL(SUM(VagasReservadas), 0) AS Reservadas
                FROM RESERVAS
                WHERE Data = @Data AND Horario = @Horario";

            using var cmd = new SqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@Data", data.Date);
            cmd.Parameters.AddWithValue("@Horario", horario);

            await _connection.OpenAsync();
            int reservadas = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            await _connection.CloseAsync();

            int vagasDisponiveis = VagasPorHorario - reservadas;

            bool disponivel = tipoUsuarioId == 2
                ? vagasDisponiveis == VagasPorHorario // Agência só se horário estiver 100% livre
                : vagasDisponiveis >= quantidade;     // Família precisa ter vagas suficientes

            if (disponivel)
            {
                resultado.Add(new
                {
                    Horario = horario,
                    VagasDisponiveis = vagasDisponiveis
                });
            }
        }

        return Ok(resultado);
    }
}
