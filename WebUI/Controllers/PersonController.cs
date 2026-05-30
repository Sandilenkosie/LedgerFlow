using Application.Commands;
using Application.Queries;
using Application.ViewModels;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using WebUI.Helpers;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePerson([FromForm] Guid id)
        {
            try
            {
                var cmd = new Application.Commands.DeletePersonCommand(id);
                var result = await _sender.Send(cmd);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = result });
                }

                TempData["ToastMessage"] = "Person removed.";
                TempData["ToastType"] = "success";
                return RedirectToAction("People");
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = $"{ex.GetType().Name}: {ex.Message}";
                TempData["ToastType"] = "error";
                return RedirectToAction("People");
            }
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
        public async Task<IActionResult> AddAccount([FromForm] Guid userId, [FromForm] decimal Balance = 0, [FromForm] string AccountType = "")
        {
            try
            {
                var cmd = new CreateAccountCommand(userId, Balance, AccountType ?? string.Empty);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeTransaction([FromForm] Guid? Id, [FromForm] Guid AccountId, [FromForm] decimal Amount, [FromForm] DateTime TransactionDate, [FromForm] string Description)
        {
            try
            {
                if (Id != null && Id != Guid.Empty)
                {
                    var updateCmd = new Application.Commands.UpdateTransactionCommand(Id.Value, AccountId, Amount, TransactionDate, Description ?? string.Empty);
                    var result = await _sender.Send(updateCmd);
                }
                else
                {
                    var cmd = new CreateTransactionCommand(AccountId, Amount, TransactionDate, Description ?? string.Empty);
                    var result = await _sender.Send(cmd);
                }
                TempData["ToastMessage"] = "Transaction created successfully.";
                TempData["ToastType"] = "success";
                ViewBag.ToastMessage = "Transaction created successfully.";
                ViewBag.ToastType = "success";
                return RedirectToAction("Transactions");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["ToastMessage"] = $"{ex.GetType().Name}: {ex.Message}";
                TempData["ToastType"] = "error";
                ViewBag.ToastMessage = $"{ex.GetType().Name}: {ex.Message}";
                ViewBag.ToastType = "error";
                return View("Transactions");
            }
        }

        public IActionResult Transactions()
        {
            // Load accounts and flatten their related transactions for the transactions view
            try
            {
                var accounts = _sender.Send(new GetAccountsQuery()).Result;

                var transactions = accounts
                    .SelectMany(a => a.Transactions.Select(t => new TransactionViewModel
                    {
                        Id = t.Id,
                        Amount = t.Amount,
                        TransactionDate = t.TransactionDate,
                        AccountId = t.AccountId,
                        AccountNumber = a.AccountNumber
                    }))
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();

                return View(transactions);
            }
            catch
            {
                // On error, return an empty list to the view
                return View(new List<TransactionViewModel>());
            }
        }

        [HttpGet]
        public IActionResult UpdatePerson()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePerson(PersonViewModel person)
        {
            try
            {
                var cmd = new PersonCommand(person);
                var result = await _sender.Send(cmd);
                TempData["ToastMessage"] = "Person updated.";
                TempData["ToastType"] = "success";
                return RedirectToAction("People");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var toastMsg = $"{ex.GetType().Name}: {ex.Message}";
                TempData["ToastMessage"] = toastMsg;
                TempData["ToastType"] = "error";
                return RedirectToAction("People");
            }
        }
    }
}
