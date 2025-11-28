
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillagioApi.Data;
using VillagioApi.DTO;
using VillagioApi.Model;

namespace VillagioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly DBContext _context;
        private const int VagasPorHorario = 50;

        public ReservasController(DBContext context)
        {
            _context = context;
        }

        [HttpGet("disponibilidade")]
        public async Task<IActionResult> GetDisponibilidade(DateTime data, int quantidade, int tipoUsuarioId)
        {
            var horarios = await _context.Horarios.Select(h => h.Hora).ToListAsync();
            var resultado = new List<object>();

            foreach (var horario in horarios)
            {
                int reservadas = await _context.Reservas
                    .Where(r => r.DataVisita == data.Date && r.HorarioVisita == horario)
                    .SumAsync(r => (int?)r.QuantidadeVisitantes) ?? 0;

                int vagasDisponiveis = VagasPorHorario - reservadas;

                bool disponivel = tipoUsuarioId == 2
                    ? vagasDisponiveis == VagasPorHorario
                    : vagasDisponiveis >= quantidade;

                if (disponivel)
                {
                    resultado.Add(new { Horario = horario, VagasDisponiveis = vagasDisponiveis });
                }
            }

            return Ok(resultado);
        }

        [HttpPost("reservar")]
        public async Task<IActionResult> Reservar([FromBody] ReservaRequest request)
        {
            if (request == null || request.Data == default || string.IsNullOrWhiteSpace(request.Horario))
                return BadRequest(new { error = "Dados inválidos." });

            int reservadas = await _context.Reservas
                .Where(r => r.DataVisita == request.Data.Date && r.HorarioVisita == request.Horario)
                .SumAsync(r => (int?)r.QuantidadeVisitantes) ?? 0;

            int vagasDisponiveis = VagasPorHorario - reservadas;

            bool podeReservar = request.TipoUsuarioId == 2
                ? vagasDisponiveis == VagasPorHorario
                : vagasDisponiveis >= request.VagasReservadas;

            if (!podeReservar)
                return Conflict(new
                {
                    error = "Não há vagas suficientes para este horário.",
                    vagasDisponiveis
                });

            var reserva = new Reserva
            {
                DataVisita = request.Data.Date,
                HorarioVisita = request.Horario,
                QuantidadeVisitantes = request.VagasReservadas,
                TipoUsuarioId = request.TipoUsuarioId
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Reserva confirmada com sucesso!",
                reserva,
                vagasRestantes = vagasDisponiveis - request.VagasReservadas
            });
        }
    }
}
