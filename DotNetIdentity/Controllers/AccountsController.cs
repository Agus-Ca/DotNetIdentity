using DotNetIdentity.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DotNetIdentity.Controllers;

public class AccountsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());
}