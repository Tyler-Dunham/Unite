using Discord_Bot_Tutorial.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discord_Bot_Tutorial.Context
{
    public class SqliteContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Queue> playerQueue { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=SqliteDB.db");
    }
}
