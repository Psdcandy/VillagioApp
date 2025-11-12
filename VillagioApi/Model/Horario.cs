using System.ComponentModel.DataAnnotations;

namespace VillagioApi.Model
{
    public class Horario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFim { get; set; }

        public bool DisponivelFamilia { get; set; } = true;

        public bool DisponivelAgencia { get; set; } = true;
    }
}