using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class DBContext :DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) :base(options)
        {

        }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<UserInfo> Department { get; set; }
        
    }
}
