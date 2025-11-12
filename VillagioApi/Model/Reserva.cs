using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillagioApi.Model
{

    public class Reserva
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataReserva { get; set; }
        public int HorarioId { get; set; }
        public int Quantidade { get; set; }
        public Usuario? Usuario { get; set; }
        public Horario? Horario { get; set; }
    }

}