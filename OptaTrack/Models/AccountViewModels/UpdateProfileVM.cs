using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptaTrack.Models.AccountViewModels
{
    public class UpdateProfileVM : ResultMessage
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string PhoneNumber { get; set; }


    }
}
