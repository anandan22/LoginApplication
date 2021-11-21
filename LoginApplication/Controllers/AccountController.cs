using LoginApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ScottBrady91.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;


namespace LoginApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signManager;


        private BCryptPasswordHasherOptions options = new BCryptPasswordHasherOptions();
        private BCryptPasswordHasher<string> CreateSut() =>
            new BCryptPasswordHasher<string>(options != null ? new OptionsWrapper<BCryptPasswordHasherOptions>(options) : null);


        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel user)
        {
            var sut = CreateSut();

            string hashedPassword=  sut.HashPassword("",user.Password);

            var _identityUser = new IdentityUser
            {
                UserName = user.Email,
                PasswordHash = hashedPassword
            };
            if(ModelState.IsValid)
            {
               var result = await _userManager.CreateAsync(_identityUser);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(user);
          
        }


        

        [HttpPost]
        [AllowAnonymous]
       public async Task<IActionResult> Login(LoginViewModel loginUser)
        {
            if(ModelState.IsValid)
            {

                var sut = CreateSut();

              // string hashedPassword = sut.HashPassword("", user.Password);


                var result = await _signManager.PasswordSignInAsync(loginUser.Email,loginUser.Password, loginUser.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(loginUser);
        }
    }
}
