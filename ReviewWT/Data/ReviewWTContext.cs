using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReviewWT.Models.CodeFirst;
using ReviewWT.Models;

namespace ReviewWT.Data
{
    public class ReviewWTContext : DbContext
    {
        public ReviewWTContext (DbContextOptions<ReviewWTContext> options)
            : base(options)
        {
        }

        public DbSet<ReviewWT.Models.CodeFirst.ExampleItem> ExampleItem { get; set; }
    }
}
