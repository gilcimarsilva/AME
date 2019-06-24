using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore;


namespace Ame.Models
{
    public class PlanetaDbContext : DbContext
    {
       

        public PlanetaDbContext(DbContextOptions<PlanetaDbContext> options)
            : base(options)
        {
        }
        public DbSet<Planeta> planetas { get; set; }        

    }

}