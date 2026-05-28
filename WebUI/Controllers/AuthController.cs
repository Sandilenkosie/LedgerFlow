using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.ViewModels;

namespace WebUI.Controllers;

public class AuthController : Controller
{
    private readonly ISender _sender;
    public AuthController(ISender sender) { _sender = sender; }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        var cmd = new LoginCommand(vm);
        var result = await _sender.Send(cmd);
        // For demo, set token in cookie
        Response.Cookies.Append("AuthToken", result.Token);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        var cmd = new RegisterCommand(vm);
        var result = await _sender.Send(cmd);
        Response.Cookies.Append("AuthToken", result.Token);
        return RedirectToAction("Index", "Home");
    }
}
