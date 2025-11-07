using Microsoft.EntityFrameworkCore;


namespace VillagioApp.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Preco> Precos { get; set; }

        // Adicione outros DbSet conforme suas tabelas

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações adicionais se necessário
        }
    }
}