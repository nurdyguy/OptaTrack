﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptaTrack.Models.AccountViewModels
{
    public class LoggedOutVM
    {
        public bool AutomaticRedirectAfterSignOut { get; set; }
        public string PostLogoutRedirectUri { get; set; }

        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }


        public string LogoutId { get; set; }
        public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;
        public string ExternalAuthenticationScheme { get; set; }
    }
}