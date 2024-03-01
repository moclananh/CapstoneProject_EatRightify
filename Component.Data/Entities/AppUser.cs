using Component.Data.Enums.UserDetailEnums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public string? VIP { get; set; }
        public decimal? AccumulatedPoints { get; set; }
        public bool IsBanned { get; set; }
        public string? RefeshToken { get; set; }
        public string? RefeshCode { get; set; }
        public DateTime? RefeshTokenExpire { get; set; }
        public string? VerifyCode { get; set; }
        public bool? IsVerify { get; set; }
        public string? Avatar { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? RefeshTokenTime { get; set; }
        public bool AcceptedTermOfUse { get; set; }
        public List<Cart> Carts { get; set; }
        public List<Order> Orders { get; set; }
        public List<Transaction> Transactions { get; set; }
        public AppUserDetails? UserDetails { get; set; }
        public List<Result>? Results { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<Slide>? Slides { get; set; }
        public List<Blog>? Blogs { get; set; }
        public List<Promotion>? Promotions { get; set; }
        public List<Location>? Locations { get; set; }

    }
}
