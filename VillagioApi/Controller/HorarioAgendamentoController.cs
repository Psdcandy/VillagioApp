
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using VillagioApi.Data;
using VillagioApi.DTO;
using VillagioApi.Model;

namespace VillagioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorariosAgendamentoController : ControllerBase
    {
        private readonly DBContext _context;

        public HorariosAgendamentoController(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna horários disponíveis para uma data.
        /// </summary>
        [HttpGet("disponibilidade")]
        public async Task<ActionResult<List<HorarioDisponivelDTO>>> GetDisponibilidade(DateTime data, int quantidade, int tipoUsuarioId)
        {
            var horarios = await _context.Horarios.ToListAsync();
            var reservas = await _context.Reservas.Where(r => r.DataVisita == data).ToListAsync();

            var lista = horarios.Select(h =>
            {
                var ocupadas = reservas.Where(r => r.HorarioVisita == h.Hora).Sum(r => r.QuantidadeVisitantes);
                var vagas = h.CapacidadeMaxima - ocupadas;
                return new HorarioDisponivelDTO
                {
                    Horario = h.Hora,
                    VagasDisponiveis = vagas
                };
            }).ToList();

            // Filtra conforme tipo de usuário
            if (tipoUsuarioId == 2) // Agência
                lista = lista.Where(x => x.VagasDisponiveis >= 50).ToList();
            else
                lista = lista.Where(x => x.VagasDisponiveis >= quantidade).ToList();

            return Ok(lista);
        }

        /// <summary>
        /// Cria uma nova reserva.
        /// </summary>
        [HttpPost("reservar")]
        public async Task<ActionResult> Reservar([FromBody] Reserva reserva)
        {
            if (reserva == null)
                return BadRequest("Dados inválidos.");

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reserva confirmada" });
        }

        /// <summary>
        /// Lista todas as reservas (admin).
        /// </summary>
        [HttpGet("listar")]
        public async Task<ActionResult<List<Reserva>>> ListarReservas()
        {
            return Ok(await _context.Reservas.ToListAsync());
        }
    }
}
