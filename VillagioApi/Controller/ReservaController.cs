using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillagioApi.Data;
using VillagioApi.Model;

namespace VillagioApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly DBContext _context;

        public ReservasController(DBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarReserva([FromBody] Reserva reserva)
        {
            var usuario = await _context.Usuarios.FindAsync(reserva.UsuarioId);
            if (usuario == null) return BadRequest("Usuário não encontrado.");

            var horario = await _context.Horarios.FindAsync(reserva.HorarioId);
            if (horario == null) return BadRequest("Horário inválido.");

            // Validações para Família
            if (usuario.TipoUsuarioId == 1) // Família
            {
                if (reserva.DataReserva.DayOfWeek != DayOfWeek.Saturday &&
                    reserva.DataReserva.DayOfWeek != DayOfWeek.Sunday)
                    return BadRequest("Família só pode reservar aos finais de semana.");

                if (horario.HoraInicio == TimeSpan.FromHours(12))
                    return BadRequest("Família não pode reservar entre 12h e 13h.");

                var totalReservado = await _context.Reservas
                    .Where(r => r.DataReserva.Date == reserva.DataReserva.Date && r.HorarioId == reserva.HorarioId)
                    .SumAsync(r => r.Quantidade);

                if (totalReservado + reserva.Quantidade > 50)
                    return BadRequest("Limite de 50 vagas por horário atingido.");
            }

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return Ok("Reserva criada com sucesso.");
        }
    }
}