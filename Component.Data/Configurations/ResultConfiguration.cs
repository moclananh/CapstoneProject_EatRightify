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
    public class ResultConfiguration : IEntityTypeConfiguration<Result>
    {
        public void Configure(EntityTypeBuilder<Result> builder)
        {
            builder.ToTable("Results");

            // Primary key configuration
            builder.HasKey(r => r.ResultId);

            // Foreign key configuration
            builder.HasOne(r => r.User)
                .WithMany(u => u.Results)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if needed

            // Other property configurations
            builder.Property(r => r.Title).IsRequired();
            builder.Property(r => r.ResultDate).IsRequired();
            builder.Property(r => r.Description).IsRequired();
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.IsSended).IsRequired();
        }
    }

}
