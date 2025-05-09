﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping_Ha.Areas.Admin.Repository;
using Shopping_Ha.Models;
using Shopping_Ha.Models.ViewModels;

namespace Shopping_Ha.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
		private readonly IEmailSender _emailSender;
		public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel>userManager, IEmailSender emailSender)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_emailSender = emailSender;
		}
		public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl});
		}
		
		

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password,false,false);
				if (result.Succeeded)
				{
					TempData["success"] = "Đăng nhập thành công";
					var receiver = "demologin979@gmail.com";
					var subject = "Đăng nhập trên thiết bị thành công.";
					var message = "Đăng nhập thành công, trải nghiệm dịch vụ nhé.";

					await _emailSender.SendEmailAsync(receiver, subject, message);
					return Redirect(loginVM.ReturnUrl ?? "/");
				}ModelState.AddModelError("", " Username hoặc Password bị sai");
			}
			return View(loginVM);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public  async Task<IActionResult> Create(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel { UserName = user.Username,Email = user.Email};
				IdentityResult result = await _userManager.CreateAsync(newUser,user.Password); 
				if (result.Succeeded)
				{
					TempData["success"] = "Tạo User thành công";
					return Redirect("/account/login");
				}
				foreach(IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(user);
		}

		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}
	}
}
