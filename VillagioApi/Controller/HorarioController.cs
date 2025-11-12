using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillagioApi.Data;
using VillagioApi.Model;

namespace VillagioApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorariosController : ControllerBase
    {
        private readonly DBContext _context;

        public HorariosController(DBContext context)
        {
            _context = context;
        }

        [HttpGet("disponibilidade")]
        public async Task<IActionResult> GetDisponibilidade(DateTime data)
        {
            var horarios = await _context.Horarios.ToListAsync();

            var reservas = await _context.Reservas
                .Where(r => r.DataReserva.Date == data.Date)
                .GroupBy(r => r.HorarioId)
                .Select(g => new { HorarioId = g.Key, Total = g.Sum(x => x.Quantidade) })
                .ToListAsync();

            var result = horarios.Select(h => new
            {
                h.Id,
                h.HoraInicio,
                h.HoraFim,
                DisponivelFamilia = h.DisponivelFamilia && data.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
                DisponivelAgencia = h.DisponivelAgencia,
                VagasRestantes = 50 - (reservas.FirstOrDefault(r => r.HorarioId == h.Id)?.Total ?? 0)
            });

            return Ok(result);
        }
    }
}