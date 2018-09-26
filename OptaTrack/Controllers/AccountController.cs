using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using AccountService.Models;
using AccountService.Models.RequestModels;
using AccountService.Services.Contracts;

using OptaTrack.Authentication;
using OptaTrack.Models;
using OptaTrack.Models.AccountViewModels;



namespace OptaTrack.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IUserDataService _userDataService;

        public AccountController(IUserDataService userDataService)
        {
            _userDataService = userDataService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return Redirect(Authentication.AuthenticationOptions.DefaultPostSignInRedirectUrl);

            var vm = new LoginVM()
            {
                AllowRememberLogin = true,
                EnableLocalLogin = true,
                ExternalProviders = new List<ExternalProvider>()
            };

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", Authentication.AuthenticationOptions.InvalidCredentialsErrorMessage);
                return await Login(Request.Query["returnUrl"]);
            }

            if (!(await _userDataService.CheckUserPassword(model.Username, model.Password)))
            {
                ModelState.AddModelError("", Authentication.AuthenticationOptions.InvalidCredentialsErrorMessage);
                return await Login(Request.Query["returnUrl"]);
            }

            // get user object
            var user = await _userDataService.GetUserByUsername(model.Username);

            await LogUserIn(user, model.RememberLogin);

            var returnUrl = StringValues.Empty;
            if (Request.Query.TryGetValue("returnUrl", out returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("LoggedOut");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoggedOut()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Logout");

            var vm = new LoggedOutVM()
            {
                AutomaticRedirectAfterSignOut = Authentication.AuthenticationOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = Authentication.AuthenticationOptions.SignOutRedirectUrl
            };
            return View(vm);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    Email = model.Email
                };
                try
                {
                    var newUser = await _userDataService.Create(user, model.Password);
                    if (newUser != null)
                    {
                        await LogUserIn(newUser, false);

                        var returnUrl = StringValues.Empty;
                        if (Request.Query.TryGetValue("returnUrl", out returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction("MyProfile", "Account");
                    }

                }
                catch (Exception ex)
                {
                    // add errors...
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile(ResultMessage message = null)
        {
            var user = await _userDataService.GetUser(User.UserId());
            if (user != null)
            {
                var vm = new UpdateProfileVM()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };
                if (message != null)
                    AddMessageFields(vm, message);

                return View(vm);
            }
            return View(new UpdateProfileVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(UpdateProfileVM model)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName) || string.IsNullOrWhiteSpace(model.Email))
                return View(model);

            var user = await _userDataService.GetUser(User.UserId());

            var request = new UpdateUserRequestModel()
            {
                UserId = User.UserId(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            user = await _userDataService.UpdateUser(request);

            if (model.NewPassword != null)
            {
                var updatePasswordRequest = new UpdateUserPasswordRequestModel()
                {
                    UserId = user.UserId,
                    NewPassword = model.NewPassword
                };
                var success = await _userDataService.UpdateUserPassword(updatePasswordRequest);
            }

            await HttpContext.SignOutAsync();
            await LogUserIn(user, false);

            var result = new ResultMessage() { ShowMessage = true, IsError = false, Message = "Profile Updated Successfully" };
            return await MyProfile(result);
        }

        [HttpPost]
        public async Task<IActionResult> CheckEmailAvailable([FromBody] string email)
        {
            if (!IsValidEmailFormat(email))
                return Json(new { Available = false });

            var user = await _userDataService.GetUserByUsername(email);
            if (user == null || user.UserId == User.UserId())
                return Json(new { Available = true });
            else
                return Json(new { Available = false });
        }


        #region PrivateFun
        private ClaimsPrincipal GetUserPrincipal(User user)
        {
            var displayName = $"{user.FirstName} {user.LastName}";
            if (string.IsNullOrWhiteSpace(displayName))
                displayName = user.Email;

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName ?? ""));
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName ?? ""));
            claims.Add(new Claim(ClaimTypes.Name, displayName));
            claims.Add(new Claim(Authentication.AuthenticationClaims.UserIdClaim, user.UserId.ToString()));

            user.Roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));

            return new ClaimsPrincipal(new ClaimsIdentity(claims.ToArray(), Authentication.AuthenticationOptions.AuthenticationType));
        }

        private async Task LogUserIn(User user, bool rememberLogin)
        {
            var newPrincipal = GetUserPrincipal(user);

            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(Authentication.AuthenticationOptions.DefaultLoginDuration)
            };

            if (rememberLogin)
                props.ExpiresUtc = DateTimeOffset.UtcNow.Add(Authentication.AuthenticationOptions.RememberMeLoginDuration);

            await HttpContext.SignInAsync(newPrincipal, props);
        }

        private bool IsValidEmailFormat(string email)
        {
            if (email.Length > 100)
                return false;

            // pattern from: http://www.rhyous.com/2010/06/15/regular-expressions-in-cincluding-a-new-comprehensive-email-pattern/
            string pattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,15})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            Regex rex = new Regex(pattern);
            if (email != rex.Match(email).Value)
                return false;

            return true;
        }
        #endregion
    }
}
