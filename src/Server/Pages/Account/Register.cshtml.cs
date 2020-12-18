using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SimplySocial.Server.Data.Identity;
using SimplySocial.Server.Models;
using SimplySocial.Server.Core.Extensions;
using SimplySocial.Server.Core.Services;

namespace SimplySocial.Server.Pages.Account
{
    public class RegisterModel : PageModel
    {
        #region Dependency Injected Members
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailer               _emailer;
        private readonly UserManager<User>      _userManager;
        private readonly SignInManager<User>    _signInManager;
        #endregion

        public RegisterModel(
            IEmailer emailer,
            ILogger<RegisterModel> logger,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _logger         = logger;
            _emailer        = emailer;
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
                var user = new User
                {
                    Email       = Input.Email,
                    UserName    = Input.UserName,
                    FirstName   = Input.FirstName,
                    LastName    = Input.LastName
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await SendEmailConfirmation(user);
                    _logger.LogInformation($"User created new account '{Input.UserName}' with password.");

                    StatusMessage status = new StatusMessage
                    {
                        Title = "Confirm Email",
                        Type = StatusMessage.StatusType.Info,
                        Content = $"A confirmation link has been sent to {Input.Email}. Please check your inbox",
                    };

                    TempData.Set("StatusMessage", status);

                    // Redirect to Confirmation Page if Account Confirmation is Required. 
                    // Otherwise just log the user in and redirect to the return url.
                    if (!_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(ReturnUrl);
                    }
                    else return RedirectToPage("Login", new { ReturnUrl });
                }

                // Adding Creation Result Errors to Model State
                foreach (var error in result.Errors)
                    ModelState.AddModelError(String.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        #endregion

        #region Private Methods

        private async Task SendEmailConfirmation(User newUser)
        {
            var userId              = await _userManager.GetUserIdAsync(newUser);
            var confirmationToken   = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var encodedToken        = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
            var confirmationLink    = Url.Action("ConfirmEmail", "Account", new { userId, token = encodedToken }, Request.Scheme, Request.Host.ToString());
            await _emailer.SendEmailConfirmationAsync(Input.Email, Input.UserName, HtmlEncoder.Default.Encode(confirmationLink));
        }

        #endregion

        #region Data Models

        public class InputModel
        {
            [Display(Name = "Username"), Required]
            [Remote("VerifyUserName", "Account", HttpMethod = "POST", ErrorMessage = "Username Taken")]
            public String UserName { get; set; }

            [Required, EmailAddress]
            [Display(Name = "Email")]
            [Remote("VerifyEmail", "Account", HttpMethod = "Post", ErrorMessage = "Email already Registered")]
            public String Email { get; set; }

            [Display(Name = "First Name"), MaxLength(50)]
            public String FirstName { get; set; }

            [Display(Name = "Last Name"), MaxLength(50)]
            public String LastName { get; set; }

            [Display(Name = "Password")]
            [Required, DataType(DataType.Password)]
            public String Password { get; set; }

            [Display(Name = "Confirm Password")]
            [Required, DataType(DataType.Password), Compare("Password")]
            public String Password2 { get; set; }
        }

        #endregion
    }
}
