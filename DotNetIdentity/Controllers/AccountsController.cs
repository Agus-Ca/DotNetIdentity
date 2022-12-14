using DotNetIdentity.Models;
using DotNetIdentity.ViewModels;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DotNetIdentity.Controllers;

public class AccountsController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public AccountsController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                UserName = registerViewModel.Email,
                Name = registerViewModel.UserName,
                Birthdate = (DateTime)registerViewModel.Birthdate,
                Email = registerViewModel.Email,
                PhoneNumber = registerViewModel.PhoneNumber,
                Url = registerViewModel.Url,
                Country = registerViewModel.Country,
                CountryCode = registerViewModel.CountryCode,
                City = registerViewModel.City,
                Address = registerViewModel.Address,
                State = registerViewModel.State
            };
            var resultado = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (resultado.Succeeded)
            {
                // Mail confirmation
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmEmailUrl = Url.Action("EmailConfirmation", "Accounts", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(registerViewModel.Email, "Confirmar cuenta - DotNetIdentity",
                    "Por favor, confirme su cuenta dando <a href=\"" + confirmEmailUrl + "\">click aqui!</a>");

                // Register
                await _signInManager.SignInAsync(user, isPersistent: false);

                return LocalRedirect(returnUrl);
            }

            ValidateErrors(resultado);
        }

        return View(registerViewModel);
    }

    // errors handler
    private void ValidateErrors(IdentityResult identityResult)
    {
        foreach (var error in identityResult.Errors)
        {
            ModelState.AddModelError(String.Empty, error.Description);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var resultado = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: true);

            if (resultado.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else if (resultado.IsLockedOut)
            {
                return View("Blocked");
            } 
            else
            {
                ModelState.AddModelError(String.Empty, "Acceso invalido");
                return View(loginViewModel);
            }
        }
        return View(loginViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpGet]
    public IActionResult ForgottenPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgottenPassword(ForgottenPasswordViewModel forgottenPasswordViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(forgottenPasswordViewModel.Email);
            if (user == null)
            {
                return RedirectToAction("ConfirmPassword");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var returnUrl = Url.Action("ResetPassword", "Accounts", new{userId = user.Id, code}, protocol: HttpContext.Request.Scheme);
           
            await _emailSender.SendEmailAsync(forgottenPasswordViewModel.Email, "Recuperar contraseña - DotNetIdentity", 
                "Por favor, recupere su contraseña dando <a href=\"" + returnUrl + "\">click aqui!</a>");

            return RedirectToAction("ConfirmPassword");
        }

        return View(forgottenPasswordViewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ConfirmPassword()
    {
        return View();
    }

    // Password recovery
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string code=null)
    {
        return code == null ? View("Error") : View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(PasswordRecoveryViewModel passwordRecoveryViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(passwordRecoveryViewModel.Email);
            if (user == null)
            {
                return RedirectToAction("ConfirmPasswordRecovery");
            }

            var result = await _userManager.ResetPasswordAsync(user, passwordRecoveryViewModel.Code, passwordRecoveryViewModel.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ConfirmPasswordRecovery");
            }

            ValidateErrors(result);
        }

        return View(passwordRecoveryViewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ConfirmPasswordRecovery()
    {
        return View();
    }

    // Email confirmation

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> EmailConfirmation(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return View("Error");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View("Error");
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        return View(result.Succeeded ? "EmailConfirmation" : "Error");
    }

    // External logins
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalAccess(string provider, string returnUrl = null)
    {
        var redirectToUrl = Url.Action("ExternalAccessCallback", "Accounts", new { ReturnUrl = returnUrl});
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectToUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalAccessCallback(string returnUrl = null, string error = null)
    {
        returnUrl ??= Url.Content("~/");
        if (error != null)
        {
            ModelState.AddModelError(string.Empty, $"External access error: {error}");
            return View(nameof(Login));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        // Access
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (result.Succeeded)
        {
            // Update tokens
            await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
            return LocalRedirect(returnUrl);
        }
        else
        {
            // User do not have account
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["ProviderDisplayName"] = info.ProviderDisplayName;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            return View("ExternalAccessConfirmation", new ExternalAccessConfirmationViewModel { Email = email, Name = name });
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalAccessConfirmation(ExternalAccessConfirmationViewModel externalAccessConfirmationViewModel, string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var information = await _signInManager.GetExternalLoginInfoAsync();
            if (information == null)
            {
                return View("Error");
            }

            var user = new AppUser { 
                UserName = externalAccessConfirmationViewModel.Email, 
                Email = externalAccessConfirmationViewModel.Email,
                Name = externalAccessConfirmationViewModel.Name,
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, information);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(information);
                    return LocalRedirect(returnUrl);
                }
            }

            ValidateErrors(result);
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View(externalAccessConfirmationViewModel);
    }
}