using Application.Commands;
using Application.Queries;
using Application.ViewModels;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Drawing;

namespace WebUI.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        private readonly ISender _sender;
        public PersonController(ISender sender) { _sender = sender; }
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> People([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            return View(await _sender.Send(new GetPersonsQuery(page, size)));
        }
        public async Task<IActionResult> Accounts()
        {
            return View(await _sender.Send(new GetAccountsQuery()));

        }

        [HttpGet]
        public async Task<IActionResult> AddAccountModal()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAccount([FromForm] Guid userId, [FromForm] decimal Balance = 0)
        {
            try
            {
                var cmd = new CreateAccountCommand(userId, Balance);
                var result = await _sender.Send(cmd);
                TempData["ToastMessage"] = "Account created successfully.";
                TempData["ToastType"] = "success";
                return RedirectToAction("People");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["ToastMessage"] = $"{ex.GetType().Name}: {ex.Message}";
                TempData["ToastType"] = "error";
                return RedirectToAction("Accounts");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddTransationModal()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MakeTransaction([FromForm] Guid AccountId, [FromForm] decimal Amount, [FromForm] string Description)
        {
            try
            {
                var cmd = new CreateTransactionCommand(AccountId, Amount, Description ?? string.Empty);
                var result = await _sender.Send(cmd);
                TempData["ToastMessage"] = "Transaction created successfully.";
                TempData["ToastType"] = "success";
                return RedirectToAction("People");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["ToastMessage"] = $"{ex.GetType().Name}: {ex.Message}";
                TempData["ToastType"] = "error";
                return RedirectToAction("Transactions");
            }
        }

        public IActionResult Transactions()
        {
            return View(new List<TransactionViewModel>());
        }

        [HttpGet]
        public IActionResult UpdatePerson()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePerson(PersonViewModel person)
        {
            try
            {
                var cmd = new PersonCommand(person);
                var result = await _sender.Send(cmd);
                return RedirectToAction("Dashboard", "Person");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var toastMsg = $"{ex.GetType().Name}: {ex.Message}";
                ViewData["ToastMessage"] = toastMsg;
                ViewData["ToastType"] = "error";
                return View(person);
            }
        }
    }
}
