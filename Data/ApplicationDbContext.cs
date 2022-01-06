using Microsoft.EntityFrameworkCore;
using api_rest.Models;

namespace api_rest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Produto> Produtos {get; set;}
        public DbSet<Usuario> Usuarios {get; set;}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
    }
}