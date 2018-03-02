using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScottBrady91.PasswordlessAuthentication.Models;

namespace ScottBrady91.PasswordlessAuthentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public AccountController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // bootstrap user
            if (!userManager.Users.Any())
                await userManager.CreateAsync(new IdentityUser
                {
                    UserName = "scott@scottbrady91.com",
                    Email = "scott@scottbrady91.com"
                });

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.EmailAddress);

            if (user == null)
            {
                // email user
            }
            else
            {
                var token = await userManager.GenerateUserTokenAsync(user, "Default", "passwordless-auth");
                var url = Url.Action("LoginCallback", "Account", new {token = token, email = model.EmailAddress}, Request.Scheme);
                System.IO.File.WriteAllText("passwordless.txt", url);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> LoginCallback(string token, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            var isValid = await userManager.VerifyUserTokenAsync(user, "Default", "passwordless-auth", token);

            if (isValid)
            {
                await HttpContext.SignInAsync(
                    IdentityConstants.ApplicationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(
                        new List<Claim> {new Claim("sub", user.Id)},
                        IdentityConstants.ApplicationScheme)));
                return RedirectToAction("Index", "Home");
            }

            return View("Error");
        }
    }
}