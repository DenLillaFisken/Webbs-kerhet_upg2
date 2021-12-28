#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webbsäkerhet_upg2.Models;

namespace Webbsäkerhet_upg2.Data
{
    public class Webbsäkerhet_upg2Context : DbContext
    {
        public Webbsäkerhet_upg2Context (DbContextOptions<Webbsäkerhet_upg2Context> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comment { get; set; }
        public DbSet<SavedFile> SavedFile { get; set; }
    }
}
