using Microsoft.EntityFrameworkCore;
using DBModel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBModel
{
    public class EFCoreDbContext : DbContext
    {
        public static string ConnStr { get; set; }
        public EFCoreDbContext()
        { }
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options) : base(options)
        { }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<Department> Department { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnStr);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }
    }
}
