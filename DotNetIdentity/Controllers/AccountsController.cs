using DotNetIdentity.Models;
using DotNetIdentity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

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
            var usuario = new AppUser
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
            var resultado = await _userManager.CreateAsync(usuario, registerViewModel.Password);

            if (resultado.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: false);

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
            var returnUrl = Url.Action("Reset password", "Accounts", new{userId = user.Id, code}, protocol: HttpContext.Request.Scheme);
           
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
}