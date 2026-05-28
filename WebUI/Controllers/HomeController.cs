using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;

namespace WebUI.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _http;
    public HomeController(IHttpClientFactory http) => _http = http;

    public async Task<IActionResult> Index()
    {
        var client = _http.CreateClient("api");
        List<object>? persons = null;
        try
        {
            persons = await client.GetFromJsonAsync<List<object>>("/api/Persons");
        }
        catch { /* ignore errors for demo */ }

        ViewData["Persons"] = persons;
        return View();
    }
}
