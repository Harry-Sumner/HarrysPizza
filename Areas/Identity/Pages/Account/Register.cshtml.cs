﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using HarrysPizza.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using HarrysPizza.Data;
using Microsoft.EntityFrameworkCore;

namespace HarrysPizza.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<HarrysPizzaUser> _signInManager;
        private readonly UserManager<HarrysPizzaUser> _userManager;
        private readonly IUserStore<HarrysPizzaUser> _userStore;
        private readonly IUserEmailStore<HarrysPizzaUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        private HarrysPizzaContext _db;
        public CheckoutCustomer Customer = new CheckoutCustomer(); 
        public Basket Basket = new Basket();

        public RegisterModel(
            UserManager<HarrysPizzaUser> userManager,
            IUserStore<HarrysPizzaUser> userStore,
            SignInManager<HarrysPizzaUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            HarrysPizzaContext db)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>

            // Code adapted from Asaduzzaman(2015)

            [Required, DataType(DataType.Text), Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required, DataType(DataType.Text), Display(Name = "Surname")]
            public string Surname { get; set; }

            // End of code adapted

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        // sets validation on data entry.

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.FirstName = Input.FirstName;
                user.Surname = Input.Surname;
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer"); //Assign default customer role to user
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        NewBasket(); //Creates newBasketID for user calling function
                        NewCustomer(Input.Email); //Assign basket to user using function
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public void NewBasket()
        {
            var currentBasket = _db.Basket.FromSqlRaw("SELECT * FROM BASKET")
                .OrderByDescending(b => b.BasketID)
                .FirstOrDefault();
            if (currentBasket == null) //if no basket, create one
            {
                Basket.BasketID = 1;
            }
            else
            {
                Basket.BasketID = currentBasket.BasketID + 1; //increment last basket by 1
            }

            _db.Basket.Add(Basket); //Assign and save changes
            _db.SaveChanges();
            // function that creates a new basket in basket table
        }


        public void NewCustomer(string Email)
        {
            Customer.Email = Input.Email;
            Customer.Name = $"{Input.FirstName} {Input.Surname}";
            Customer.BasketID = Basket.BasketID; // links user account to basket
            _db.CheckoutCustomers.Add(Customer); 
            _db.SaveChanges();
            // function that assigns basket to user and saves to database
        }

        private HarrysPizzaUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<HarrysPizzaUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(HarrysPizzaUser)}'. " +
                    $"Ensure that '{nameof(HarrysPizzaUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<HarrysPizzaUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<HarrysPizzaUser>)_userStore;
        }
    }
}
