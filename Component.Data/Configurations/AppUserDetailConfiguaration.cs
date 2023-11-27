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

    public class AppUserDetailConfiguaration : IEntityTypeConfiguration<AppUserDetails>
    {
        public void Configure(EntityTypeBuilder<AppUserDetails> builder)
        {
            builder.ToTable("AppUserDetails");

            // Primary key configuration
            builder.HasKey(ad => ad.UserId);

            // Foreign key configuration
            builder.HasOne(ad => ad.User)
                .WithOne(u => u.UserDetails)
                .HasForeignKey<AppUserDetails>(ad => ad.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if needed

            // Other property configurations
            builder.Property(ad => ad.Gender).IsRequired();
            builder.Property(ad => ad.AgeRange).IsRequired();
            builder.Property(ad => ad.Goal).IsRequired();
            builder.Property(ad => ad.BodyType).IsRequired();
            builder.Property(ad => ad.BodyGoal).IsRequired();
            builder.Property(ad => ad.TagetZone).IsRequired();
            builder.Property(ad => ad.TimeSpend).IsRequired();
            builder.Property(ad => ad.LastPerfectWeight).IsRequired();
            builder.Property(ad => ad.DoWorkout).IsRequired();
            builder.Property(ad => ad.FeelTired).IsRequired();
            builder.Property(ad => ad.Height).IsRequired();
            builder.Property(ad => ad.CurrentWeight).IsRequired();
            builder.Property(ad => ad.GoalWeight).IsRequired();
            builder.Property(ad => ad.TimeSleep).IsRequired();
            builder.Property(ad => ad.WaterDrink).IsRequired();
            builder.Property(ad => ad.Diet).IsRequired();
            builder.Property(ad => ad.ProductAllergies).IsRequired(false);

        }
    }
}
