
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillagioApi.Model
{

    [Table("HORARIOS")]
    public class Horario
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(5)]
        [Column("Horario")]
        public string Hora { get; set; } = string.Empty; // Ex: "08:00"

        [Required]
        [Column("CapacidadeMaxima")]
        public int CapacidadeMaxima { get; set; } = 50; // Padrão 50 vagas
    }

}
