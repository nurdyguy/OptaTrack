using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptaTrack.Authentication
{
    public static class AuthenticationOptions
    {

        public static string InvalidCredentialsErrorMessage = "Invalid username or password";

        public static bool AutomaticRedirectAfterSignOut = true;
        public static string SignOutRedirectUrl = "/";
        public static string DefaultPostSignInRedirectUrl = "/";
        public static string AuthenticationType = "cookie";

        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public static TimeSpan DefaultLoginDuration = TimeSpan.FromDays(1);

        // unused in core 2.0
        //public static readonly string AuthScheme = "sbx.scheme";
        //public static readonly string UserSubjectPrefix = "user";
        //public static bool ShowLogoutPrompt = false;

        // external auto options
        //public static readonly SupportedExternalProvider Google = new SupportedExternalProvider
        //{
        //    AuthenticationScheme = "Google",
        //    Authority = "Google"
        //};

        //public static readonly SupportedExternalProvider Facebook = new SupportedExternalProvider
        //{
        //    AuthenticationScheme = "Facebook",
        //    Authority = ""
        //};

        //public static List<SupportedExternalProvider> Providers = new List<SupportedExternalProvider> { Google, Facebook };

        //public static class Scopes
        //{
        //    public const string Roles = "roles";
        //    public const string Role = "role";
        //}

        //public static readonly Dictionary<string, IEnumerable<string>> ScopeToClaimsMapping = new Dictionary<string, IEnumerable<string>>
        //{
        //    {
        //        Scopes.Role,
        //        new[]
        //        {
        //            JwtClaimTypes.Role
        //        }
        //    }
        //};

        //public static class Roles
        //{
        //    public const string Software = "Software";
        //}

        //public class Filters
        //{
        //    // filter for claims from an incoming access token (e.g. used at the user profile endpoint)
        //    public static readonly string[] ProtocolClaimsFilter = new string[]
        //    {
        //        JwtClaimTypes.AccessTokenHash,
        //        JwtClaimTypes.Audience,
        //        JwtClaimTypes.AuthenticationTime,
        //        JwtClaimTypes.AuthorizedParty,
        //        JwtClaimTypes.AuthorizationCodeHash,
        //        JwtClaimTypes.ClientId,
        //        JwtClaimTypes.Expiration,
        //        JwtClaimTypes.IssuedAt,
        //        JwtClaimTypes.Issuer,
        //        JwtClaimTypes.JwtId,
        //        JwtClaimTypes.Nonce,
        //        JwtClaimTypes.NotBefore,
        //        JwtClaimTypes.ReferenceTokenId,
        //        JwtClaimTypes.SessionId,
        //        JwtClaimTypes.Scope,
        //    };

        //    // filter list for claims returned from profile service prior to creating tokens
        //    public static readonly string[] ClaimsServiceFilterClaimTypes = new string[]
        //    {
        //        // TODO: consider JwtClaimTypes.AuthenticationContextClassReference,
        //        JwtClaimTypes.AccessTokenHash,
        //        JwtClaimTypes.Audience,
        //        JwtClaimTypes.AuthenticationMethod,
        //        JwtClaimTypes.AuthenticationTime,
        //        JwtClaimTypes.AuthorizedParty,
        //        JwtClaimTypes.AuthorizationCodeHash,
        //        JwtClaimTypes.ClientId,
        //        JwtClaimTypes.Expiration,
        //        JwtClaimTypes.IdentityProvider,
        //        JwtClaimTypes.IssuedAt,
        //        JwtClaimTypes.Issuer,
        //        JwtClaimTypes.JwtId,
        //        JwtClaimTypes.Nonce,
        //        JwtClaimTypes.NotBefore,
        //        JwtClaimTypes.ReferenceTokenId,
        //        JwtClaimTypes.SessionId,
        //        JwtClaimTypes.Subject,
        //        JwtClaimTypes.Scope,
        //        JwtClaimTypes.Confirmation
        //    };
        //}
    }

    public class SupportedExternalProvider
    {
        public string AuthenticationScheme { get; set; }
        public string Authority { get; set; }
    }
}
