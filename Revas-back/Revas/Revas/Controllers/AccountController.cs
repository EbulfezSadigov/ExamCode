using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Revas.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Revas.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<IActionResult> Register()
        {
            IdentityUser user = new IdentityUser
            {
                Email="ebulfez@code.edu.az",
                UserName="Ebulfez"
            };

            IdentityResult result = await _userManager.CreateAsync(user, "Ebulfez123@");

            if (!result.Succeeded)
            {
                foreach (IdentityError item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return Content("Not Okay");
            }

            return Content("Okey");
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            IdentityUser user = await _userManager.FindByEmailAsync(model.Email);

            if (user==null)
            {
                ModelState.AddModelError("", "Email or Password is wrong");
                return View();
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, model.StayLoggedIn, false);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("","You are locked out");
                }
                else
                {
                    ModelState.AddModelError("", "Email or Password is wrong");
                }

                return View(model);
            }

            return RedirectToAction("Index","Dashboard",new {area="Admin" });
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }


    }
}
