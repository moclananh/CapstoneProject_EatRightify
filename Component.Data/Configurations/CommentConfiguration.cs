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
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");

            // Primary key configuration
            builder.HasKey(c => c.Id);

            // Foreign key configuration
            builder.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if needed

            // Other property configurations
            builder.Property(c => c.Content).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.Status).IsRequired();
        }
    }

}
