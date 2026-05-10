using HouseBroker.Domain.Entities;
using HouseBroker.Infra.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Infra.DBContext
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser> //to get identity tables
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<PropertyListing> Properties => Set<PropertyListing>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<CommissionPrice> CommissionRates => Set<CommissionPrice>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Identity table mapping
            base.OnModelCreating(builder);

            // pulls all IEntityTypeConfiguration<T> in this assembly
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
