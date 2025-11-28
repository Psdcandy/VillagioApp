
using Microsoft.EntityFrameworkCore;
using VillagioApi.Model;

namespace VillagioApi.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Horario> Horarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Horario>(entity =>
            {
                entity.ToTable("HORARIOS");
                entity.Property(p => p.Hora)
                      .HasColumnName("Horario") // coluna do banco
                      .HasMaxLength(5)
                      .IsRequired();
            });


            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.ToTable("RESERVAS");
                entity.Property(p => p.HorarioVisita)
                      .HasColumnName("Horario")
                      .HasMaxLength(5)
                      .IsRequired();
            });

        }
    }
}