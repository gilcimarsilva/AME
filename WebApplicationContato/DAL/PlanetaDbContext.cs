using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using WebApplicationContato.Models;


namespace WebApplicationContato.DAL
{
    public class PlanetaDbContext : DbContext
    {
        public DbSet<Planeta> planetas { get; set; }        

    }

}