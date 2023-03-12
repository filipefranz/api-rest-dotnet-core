using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_rest_dotnet_core.Models;
using Microsoft.EntityFrameworkCore;

namespace api_rest_dotnet_core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Produto>? Produtos { get; set; }
        public DbSet<Usuario>? Usuarios { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    }
}