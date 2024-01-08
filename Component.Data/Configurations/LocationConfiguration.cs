using Component.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations");

            builder.HasKey(x => x.LocationId);
            builder.Property(x => x.LocationId).UseIdentityColumn();
            // Foreign key configuration
            builder.HasOne(r => r.User)
                .WithMany(u => u.Locations)
                .HasForeignKey(r => r.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
