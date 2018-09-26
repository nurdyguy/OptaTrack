using System;
using System.Collections.Generic;

namespace AccountService.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        public string Salt { get; set; }
        public string HashedPassword { get; set; }
        public string TempPassword { get; set; }

        public bool IsActive { get; set; }
        public DateTime RegisteredOn { get; set; }
        public DateTime LastLogOn { get; set; }

        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }
        public string ThirdPartyGuid { get; set; }
        public string ConcurrencyStamp { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }


        //ICollection<UserClaim> Claims { get; }
        public IEnumerable<Role> Roles { get; set; }

        //ICollection<UserLogin> Logins { get; }
        //string SecurityStamp { get; set; }
    }
}
