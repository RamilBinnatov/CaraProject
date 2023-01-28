﻿using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.Services.Inteface;
using FinalProject.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class AccountController : Controller
    {
        #region Readonly
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        #endregion

        #region Constructor
        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailService emailService,
            IFileService fileService,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _fileService = fileService;
            _roleManager = roleManager;
        }

        #endregion

        #region Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            AppUser user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);

            if (user is null)
            {
                user = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            }

            if (user is null)
            {
                ModelState.AddModelError("", "Email or password wrong");
                return View(loginVM);
            }



            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or password wrong");
                return View(loginVM);
            }


            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            AppUser user = new AppUser
            {
                FullName = registerVM.FullName,
                UserName = registerVM.Username,
                Email = registerVM.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token },
                Request.Scheme, Request.Host.ToString());

            string subject = "Verify Email";
            string path = "wwwroot/templates/Verify.html";
            string body = string.Empty;

            body = _fileService.ReadFile(path, body);

            body = body.Replace("{{link}}", link);
            body = body.Replace("{{fullname}}", user.FullName);

            _emailService.Send(user.Email, subject, body);

            return RedirectToAction(nameof(VerifyEmail));
        }
        #endregion

        #region logOut
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region ConfirmEmail&verifyEmail
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return BadRequest();

            AppUser user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            await _userManager.ConfirmEmailAsync(user, token);

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Home");

        }

        public IActionResult VerifyEmail()
        {
            return View();
        }
        #endregion

        #region ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPassword)
        {
            if (!ModelState.IsValid) return View();

            AppUser existUser = await _userManager.FindByEmailAsync(forgotPassword.Email);

            if (existUser is null)
            {
                ModelState.AddModelError("Email", "User not found");
                return View();
            }


            string token = await _userManager.GeneratePasswordResetTokenAsync(existUser);

            string link = Url.Action(nameof(ResetPassword), "Account", new { userId = existUser.Id, token },
                Request.Scheme, Request.Host.ToString());

            string path = "wwwroot/templates/Verify.html";
            string body = string.Empty;
            string subject = "Verify password reset email";

            body = _fileService.ReadFile(path, body);

            body = body.Replace("{{link}}", link);
            body = body.Replace("{{fullname}}", existUser.FullName);

            _emailService.Send(existUser.Email, subject, body);

            return RedirectToAction(nameof(VerifyEmail));

        }
        #endregion

        #region ResetPassword
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token) {
            ResetPasswordVM resetPasswordVM = new ResetPasswordVM()
            {
                Token = token,
                UserId = userId
            };
            return View(resetPasswordVM);                
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPassword)
        
        {
            if (!ModelState.IsValid) return View(resetPassword);

            AppUser existUser = await _userManager.FindByIdAsync(resetPassword.UserId);

            if (existUser == null) return NotFound();

            if (await _userManager.CheckPasswordAsync(existUser, resetPassword.Password))
            {
                ModelState.AddModelError("", "New password cant be same with old password");
                return View(resetPassword);
            }

            await _userManager.ResetPasswordAsync(existUser, resetPassword.Token, resetPassword.Password);

            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region CreateRoles
        [HttpGet]
        public async Task CreateRoles()
        {
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                }
            }

        }
        #endregion
    }
}
