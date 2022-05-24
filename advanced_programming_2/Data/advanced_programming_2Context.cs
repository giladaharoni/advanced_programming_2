using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using advanced_programming_2.Models;

namespace advanced_programming_2.Data
{
    public class advanced_programming_2Context : DbContext
    {
        public advanced_programming_2Context (DbContextOptions<advanced_programming_2Context> options)
            : base(options)
        {
        }

        public DbSet<advanced_programming_2.Models.rating>? rating { get; set; }

        public DbSet<advanced_programming_2.Models.chathistory>? chathistory { get; set; }

        public DbSet<advanced_programming_2.Models.message>? message { get; set; }

        public DbSet<advanced_programming_2.Models.Contact>? Contact { get; set; }
    }
}
