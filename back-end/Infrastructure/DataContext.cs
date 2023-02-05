using System;
using System.Collections.Generic;
using Adverthouse.Core.Configuration;
using Adverthouse.Core.Security;
using back_end.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace back_end.Infrastructure
{
    public class DataContext : DbContext
    { 
        public DbSet<Member> Members { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    
         public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public void SeedData()
        {
            Members.Add(new Member { MemberId= 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test", Avatar = "Layer3Kopya.png" });            

            SaveChanges();
        }
    }
}
