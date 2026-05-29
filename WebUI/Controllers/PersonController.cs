using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult All()
        {
            return View();
        }
    }
}
