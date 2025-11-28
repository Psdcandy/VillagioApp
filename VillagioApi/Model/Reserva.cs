using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillagioApi.Model
{


    [Table("RESERVAS")]
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public DateTime DataVisita { get; set; }

        [Required]
        [MaxLength(5)]
        public string HorarioVisita { get; set; } = string.Empty;

        [Required]
        public int QuantidadeVisitantes { get; set; }

        // Novas propriedades
        public int TipoUsuarioId { get; set; } // 1 = Família, 2 = Agência
        public int VagasReservadas { get; set; } // Para agência (50) ou família (visitantes)
    }
}
