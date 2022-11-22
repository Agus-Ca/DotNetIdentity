using DotNetIdentity.Models;
using DotNetIdentity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotNetIdentity.Controllers;

public class AccountsController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountsController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
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

                return RedirectToAction("Index", "Home");
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
}