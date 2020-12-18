using System;
using System.Text;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

using SimplySocial.Server.Data.Identity;

namespace SimplySocial.Server.Pages.Account
{
    public class LoginModel : PageModel
    {
        #region Dependency Injected Members
        private readonly ILogger<LoginModel>    _logger;
        private readonly UserManager<User>      _userManager;
        private readonly SignInManager<User>    _signInManager;
        #endregion

        public LoginModel(
            ILogger<LoginModel> logger,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _logger         = logger;
            _userManager    = userManager;
            _signInManager  = signInManager;
        }

        #region Properties
        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty(SupportsGet = true)]
        public String ReturnUrl { get; set; }
        #endregion

        #region Page Handlers

        public void OnGet() => ReturnUrl ??= Url.Content("~/");

        public async Task<IActionResult> OnPostAsync()
        {
            ReturnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var loginIsEmail = MailAddress.TryCreate(Input.UserLogin, out var tempMailAddress);
                var user = loginIsEmail
                    ? await _userManager.FindByEmailAsync(Input.UserLogin)
                    : await _userManager.FindByNameAsync(Input.UserLogin);

                var userLogin = user?.UserName ?? Input.UserLogin;

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(userLogin, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User '{user.UserName}' has logged in.");
                    return LocalRedirect(ReturnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"User account '{user.UserName}' is locked out.");
                    ModelState.AddModelError(String.Empty, "Account Locked");
                }
                else ModelState.AddModelError(String.Empty, "Invalid login attempt.");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        #endregion

        #region Data Models

        public class InputModel
        {
            [Required, Display(Name = "Username / Email")]
            public String UserLogin     { get; set; }

            [Required, DataType(DataType.Password)]
            public String Password      { get; set; }

            [Display(Name = "Remember Me?")]
            public Boolean RememberMe   { get; set; }
        }

        #endregion
    }
}
