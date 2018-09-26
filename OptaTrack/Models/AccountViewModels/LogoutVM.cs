using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptaTrack.Models.AccountViewModels
{
    public class LogoutVM : LogoutInputVM
    {
        public bool ShowLogoutPrompt { get; set; }
    }
}
