using System.ComponentModel.DataAnnotations;

namespace VillagioApi.Model
{
    public class TipoUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } // Ex: "Família", "Agência"
    }
}