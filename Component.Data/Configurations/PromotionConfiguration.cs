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
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotions");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            // Foreign key configuration
            builder.HasOne(r => r.User)
                .WithMany(u => u.Promotions)
                .HasForeignKey(r => r.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);
            //other entities
            builder.Property(x => x.Name).IsRequired();
        }
    }
}
