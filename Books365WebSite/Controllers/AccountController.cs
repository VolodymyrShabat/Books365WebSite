using Books365WebSite.Infrustructure;
using Books365WebSite.Models;
using Books365WebSite.Services;
using Books365WebSite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly UserManager<User> _UserManager;
    private readonly SignInManager<User> _SignInManager;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _UserManager = userManager;
        _SignInManager = signInManager;
    }
    [HttpGet]
    public IActionResult Register()
    {
        if (_SignInManager.IsSignedIn(User))
        {
            return Redirect("~/Budget/Index");
        }
        else
        {
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (ModelState.IsValid)
        {
            User user = new User { Email = registerViewModel.Email, UserName = registerViewModel.UserName };
            var result = await _UserManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {
                // Generate Token
                var code = await _UserManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = user.Id, code = code },
                    protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(registerViewModel.Email, "Confirm your account",
                    $"Confirm registration : <a href='{callbackUrl}'>link</a>");

                return View("Confirm");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }
        }
        return View();
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return View("Error");
        }
        var user = await _UserManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View("Error");
        }
        var result = await _UserManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
            return View("ConfirmSuccess");
        else
            return View("Error");
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        if (_SignInManager.IsSignedIn(User))
        {
            return Redirect("~/Home/Index");
        }
        else
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result =
                await _SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Wrong login and (or) password");
            }
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _SignInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [Authorize]
    public async Task<IActionResult> Edit(string id)
    {
        User user = await _UserManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        EditViewModel model = new EditViewModel { Id = user.Id, UserName = user.UserName, Email = user.Email };
        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(EditViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _UserManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                var result = await _UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
        }
        return View(model);
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _UserManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _UserManager.IsEmailConfirmedAsync(user)))
            {
                // користувача може і не бути в бд але все рівно повертаємо звичайне повідомлення
                return View("ForgotPasswordConfirmation");
            }

            var code = await _UserManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(model.Email, "Reset Password",
                $"For password reset folow the link: <a href='{callbackUrl}'>link</a>");
            return View("ForgotPasswordConfirmation");
        }
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string code = null)
    {
        return code == null ? View("Error") : View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _UserManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return View("ResetPasswordSuccess");
        }
        var result = await _UserManager.ResetPasswordAsync(user, model.Code, model.Password);
        if (result.Succeeded)
        {
            return View("ResetPasswordSuccess");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }
}
