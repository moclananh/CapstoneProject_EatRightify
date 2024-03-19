using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.System.Users
{
    public class UserUpdateRequest
    {
        public Guid Id { get; set; }   
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Dob { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
    }
}
