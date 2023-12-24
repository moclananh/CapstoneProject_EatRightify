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
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blogs");

            // Primary key configuration
            builder.HasKey(r => r.Id); ;
            builder.Property(x => x.Id).UseIdentityColumn();
            // Foreign key configuration
            builder.HasOne(r => r.User)
                .WithMany(u => u.Blogs)
                .HasForeignKey(r => r.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if needed

            // Other property configurations
            builder.Property(r => r.Title).IsRequired();
        }
    }
}
