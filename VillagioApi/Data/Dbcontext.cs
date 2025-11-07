using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VillagioApi.Model;

namespace VillagioApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
