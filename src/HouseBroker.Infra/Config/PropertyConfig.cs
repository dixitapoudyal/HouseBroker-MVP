using HouseBroker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Infra.Config
{
    public class PropertyConfig: IEntityTypeConfiguration<PropertyInfo>
    {
        public void Configure(EntityTypeBuilder<PropertyInfo> builder)
        {
            builder.ToTable("PropertyInfo");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(2000);
            builder.Property(p => p.Location).IsRequired().HasMaxLength(200);
            builder.Property(p => p.BrokerId).IsRequired();

            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

            builder.HasMany(p => p.Images)
                   .WithOne(i => i.PropertyInfo)
                   .HasForeignKey(i => i.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);

            // to implemenet search on these, indexes created.
            builder.HasIndex(p => p.BrokerId);
            builder.HasIndex(p => p.Location);
            builder.HasIndex(p => p.PropertyType);
        }
    }
}
