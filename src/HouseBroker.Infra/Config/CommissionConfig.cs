using HouseBroker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Infra.Config
{
    public class CommissionRateConfiguration : IEntityTypeConfiguration<CommissionPrice>
    {
        public void Configure(EntityTypeBuilder<CommissionPrice> builder)
        {
            builder.ToTable("CommissionPrice");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.MinPrice).HasColumnType("decimal(18,2)");
            builder.Property(r => r.MaxPrice).HasColumnType("decimal(18,2)");
            builder.Property(r => r.Rate).HasColumnType("decimal(6,4)");

            var createdOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new CommissionPrice { Id = 1, MinPrice = 0, MaxPrice = 5_000_000m, Rate = 0.0200m, CreatedOn = createdOn },
                new CommissionPrice { Id = 2, MinPrice = 5_000_000m, MaxPrice = 10_000_000m, Rate = 0.0175m,  CreatedOn = createdOn },
                new CommissionPrice { Id = 3, MinPrice = 10_000_000m, MaxPrice = null, Rate = 0.0150m, CreatedOn = createdOn }
            );
        }
    }
}
