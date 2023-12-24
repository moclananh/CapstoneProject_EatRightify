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
    public class SlideConfiguration : IEntityTypeConfiguration<Slide>
    {
        public void Configure(EntityTypeBuilder<Slide> builder)
        {
            builder.ToTable("Slides");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            // Foreign key configuration
            builder.HasOne(r => r.User)
                .WithMany(u => u.Slides)
                .HasForeignKey(r => r.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);
            //other entities
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Url).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Image).HasMaxLength(200).IsRequired();          
        }
    }
}
