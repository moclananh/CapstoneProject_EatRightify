using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.System.Users
{
    public class UserVm
    {
        public Guid Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime? Dob { get; set; }

        public IList<string> Roles { get; set; }
        public int VIP { get; set; }
        public decimal? AccumulatedPoints { get; set; }
        public bool IsBanned { get; set; }
        public string Avatar { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool AcceptedTermOfUse { get; set; }
    }
}
