using Component.Data.Entities;
using Component.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppConfig>().HasData(
               new AppConfig() { Key = "HomeTitle", Value = "This is home page of Web" },
               new AppConfig() { Key = "HomeKeyword", Value = "This is keyword of Web" },
               new AppConfig() { Key = "HomeDescription", Value = "This is description of Web" }
               );
            modelBuilder.Entity<Language>().HasData(
                new Language() { Id = "vi", Name = "Tiếng Việt", IsDefault = true },
                new Language() { Id = "en", Name = "English", IsDefault = false });

            modelBuilder.Entity<Category>().HasData(
                new Category()
                {
                    Id = 1,
                    IsShowOnHome = true,
                    ParentId = null,
                    SortOrder = 1,
                    Status = Status.Active,
                },
                 new Category()
                 {
                     Id = 2,
                     IsShowOnHome = true,
                     ParentId = null,
                     SortOrder = 2,
                     Status = Status.Active
                 },
                 new Category()
                 {
                     Id = 3,
                     IsShowOnHome = true,
                     ParentId = null,
                     SortOrder = 3,
                     Status = Status.Active
                 }
                 , new Category()
                 {
                     Id = 4,
                     IsShowOnHome = true,
                     ParentId = null,
                     SortOrder = 4,
                     Status = Status.Active
                 });

            modelBuilder.Entity<CategoryTranslation>().HasData(
                  new CategoryTranslation() { Id = 1, CategoryId = 1, Name = "Combo", LanguageId = "en", SeoAlias = "Combo", SeoDescription = "Combo weight loss meal package.", SeoTitle = "Combo" },
                  new CategoryTranslation() { Id = 2, CategoryId = 2, Name = "Weight loss foods", LanguageId = "en", SeoAlias = "Weight-loss-foods", SeoDescription = "Weight loss foods.", SeoTitle = "Weight loss foods" },
                  new CategoryTranslation() { Id = 3, CategoryId = 3, Name = "Weight gain foods", LanguageId = "en", SeoAlias = "Weight-gain-foods", SeoDescription = "Weight gain foods.", SeoTitle = "Weight gain foods" },
                  new CategoryTranslation() { Id = 4, CategoryId = 4, Name = "Discount Products", LanguageId = "en", SeoAlias = "Discount-product", SeoDescription = "Discount product ", SeoTitle = "Discount product" }
                    );

            modelBuilder.Entity<Product>().HasData(
           new Product()
           {
               Id = 1,
               DateCreated = DateTime.Now,
               OriginalPrice = 255,
               Price = 199,
               Stock = 100,
               ViewCount = 0,
           });
            modelBuilder.Entity<ProductTranslation>().HasData(
                 new ProductTranslation()
                 {
                     Id = 1,
                     ProductId = 1,
                     Name = "EAT CLEAN 7-DAY WEIGHT LOSS DIET",
                     LanguageId = "en",
                     SeoAlias = "eat-clean-7-days",
                     SeoDescription = "You say I'm fat, I lose weight. You say I'm poor, I'll make money. When I'm skinny, beautiful and rich, will I still choose you?",
                     SeoTitle = "eat clean 7 day",
                     Details = "eat clean 7 day",
                     Description = "Eat Clean weekly weight loss meal package 1 meal per day for 7 days\r\n– Low starch\r\n– Deliver meal packages to your home from Monday to Sunday\r\n– Calories range from 500 – 600 per day\r\n– Low sugar, no MSG, clean green vegetables selected from the supermarket\r\n– Provides adequate protein for the body\r\n- Suitable for those who are sedentary and often sit in the office."
                 });
            modelBuilder.Entity<ProductInCategory>().HasData(
                new ProductInCategory() { ProductId = 1, CategoryId = 1 }
                );

            // any guid
            var adminRoleId = new Guid("46f889a9-662d-4969-84f3-6ff4e199ecf5");
            var adminId = new Guid("93510e19-8812-482f-8f1b-e116cf8c9e38");
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = adminRoleId,
                Name = "admin",
                NormalizedName = "admin",
                Description = "Administrator role"
            });

            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = adminId,
                UserName = "admin",
                NormalizedUserName = "admin",
                Email = "admin@admin.com",
                NormalizedEmail = "admin@admin.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "admin"),
                SecurityStamp = string.Empty,
                FirstName = "Admin",
                LastName = "minator",
                Dob = new DateTime(2023, 01, 01)
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = adminRoleId,
                UserId = adminId
            });

            // any guid
            var managerRoleId = new Guid("05dc0e15-0df0-4b67-b76e-47ee37791bd4");
            var managerId = new Guid("648d9797-a78f-4e71-bf5d-90196c3f4806");
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = managerRoleId,
                Name = "manager",
                NormalizedName = "managger",
                Description = "Manager role"
            });

            var hasherManager = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = managerId,
                UserName = "manager",
                NormalizedUserName = "manager",
                Email = "manager@manager.com",
                NormalizedEmail = "manager@manager.com",
                EmailConfirmed = true,
                PasswordHash = hasherManager.HashPassword(null, "manager"),
                SecurityStamp = string.Empty,
                FirstName = "Manager",
                LastName = "minator",
                Dob = new DateTime(2023, 01, 01)
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = managerRoleId,
                UserId = managerId
            });

            // any guid
            var confirminatorRoleId = new Guid("07ad9a53-bb09-4d2a-ae06-89131aa9751b");
            var confiminatorId = new Guid("1ec8cb63-dc7e-492c-83b2-d02dc476061c");
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = confirminatorRoleId,
                Name = "confiminator",
                NormalizedName = "confiminator",
                Description = "Confiminator role"
            });

            var hasherConfirm = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = confiminatorId,
                UserName = "confirm",
                NormalizedUserName = "confiminator",
                Email = "confirm@confim.com",
                NormalizedEmail = "confirm@confim.com",
                EmailConfirmed = true,
                PasswordHash = hasherConfirm.HashPassword(null, "confirm"),
                SecurityStamp = string.Empty,
                FirstName = "Confirm",
                LastName = "minator",
                Dob = new DateTime(2023, 01, 01)
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = confirminatorRoleId,
                UserId = confiminatorId
            });

            modelBuilder.Entity<Slide>().HasData(
              new Slide() { Id = 1, Name = "Second Thumbnail label", Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", SortOrder = 1, Url = "#", Image = "/themes/images/carousel/1.png", Status = Status.Active },
              new Slide() { Id = 2, Name = "Second Thumbnail label", Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", SortOrder = 2, Url = "#", Image = "/themes/images/carousel/2.png", Status = Status.Active },
              new Slide() { Id = 3, Name = "Second Thumbnail label", Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", SortOrder = 3, Url = "#", Image = "/themes/images/carousel/3.png", Status = Status.Active },
              new Slide() { Id = 4, Name = "Second Thumbnail label", Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", SortOrder = 4, Url = "#", Image = "/themes/images/carousel/4.png", Status = Status.Active },
              new Slide() { Id = 5, Name = "Second Thumbnail label", Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", SortOrder = 5, Url = "#", Image = "/themes/images/carousel/5.png", Status = Status.Active },
              new Slide() { Id = 6, Name = "Second Thumbnail label", Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", SortOrder = 6, Url = "#", Image = "/themes/images/carousel/6.png", Status = Status.Active }
              );
        }
    }
}
