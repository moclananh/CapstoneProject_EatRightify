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

        public List<Cart> Carts { get; set; }

        public List<Order> Orders { get; set; }

        public List<Transaction> Transactions { get; set; }
        public bool IsBanned { get; set; }
        public AppUserDetails? UserDetails { get; set; }

    }
}
