using Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        try
        {
            var cmd = new LoginCommand(vm);
            var result = await _sender.Send(cmd);
            // For demo, set token in cookie
            Response.Cookies.Append("AuthToken", result.Token);
            return RedirectToAction("Dashboard", "Person");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var toastMsg = $"{ex.GetType().Name}: {ex.Message}";
            ViewData["ToastMessage"] = toastMsg;
            ViewData["ToastType"] = "error";
            return View(vm);
        }
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        try
        {
            var cmd = new RegisterCommand(vm);
            var result = await _sender.Send(cmd);
            Response.Cookies.Append("AuthToken", result.Token);
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var toastMsg = $"{ex.GetType().Name}: {ex.Message}";
            ViewData["ToastMessage"] = toastMsg;
            ViewData["ToastType"] = "error";
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        ViewData["ToastMessage"] = "Signed out successfully";
        ViewData["ToastType"] = "success";
        return RedirectToAction("Login", "Auth");
    }
}
