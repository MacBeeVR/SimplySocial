using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authorization;

using SimplySocial.Server.Data.Identity;

namespace SimplySocial.Server.Controllers
{
    [Route("/[controller]/[action]")]
    public class AccountController : Controller
    {
        #region Dependency Injected Members
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User>          _userManager;
        private readonly SignInManager<User>        _signInManager;
        #endregion

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger)
        {
            _logger         = logger;
            _userManager    = userManager;
            _signInManager  = signInManager;
        }

        #region Action Methods

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> Logout([FromQuery] String returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            var username = (await _userManager.GetUserAsync(HttpContext.User))?.UserName;

            await _signInManager.SignOutAsync();

            if (!String.IsNullOrWhiteSpace(username))
                _logger.LogInformation($"User {username} has logged out.");

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(String userId, String token)
        {
            if (!ParametersValid(userId, token))
                return LocalRedirect("/");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest();

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, token);
            TempData["StatusMessage"] = !result.Succeeded
                ? BuildErrorStatus(result)
                : "Email Successfully Verified! Please Log in to Continue.";


            return LocalRedirect(Url.Page("/Account/Login"));
        }

        [HttpPost]
        public async Task<Boolean> VerifyUserName([Bind(Prefix = "Input.UserName")] String UserName)
        {
            if (await _userManager.FindByNameAsync(UserName) == null)
                return true;

            return false;
        }

        [HttpPost]
        public async Task<Boolean> VerifyEmail([Bind(Prefix = "Input.Email")] String Email)
        {
            if (await _userManager.FindByEmailAsync(Email) == null)
                return true;

            return false;
        }

        #endregion

        #region Private Methods

        [NonAction]
        private Boolean ParametersValid(params String[] parameters)
        {
            Boolean isValid = true;
            for (Int32 i = 0; i < parameters.Length && isValid; i++)
                isValid = !String.IsNullOrWhiteSpace(parameters[i]);

            return isValid;
        }

        [NonAction]
        public String BuildErrorStatus(IdentityResult identityResult)
        {
            StringBuilder errorBuilder = new StringBuilder("Error: ");
            foreach (var error in identityResult.Errors)
                errorBuilder.AppendLine(error.Description);
            return errorBuilder.ToString();
        }

        #endregion
    }
}
