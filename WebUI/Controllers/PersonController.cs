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

        public PersonController(ISender sender)
        {
            _sender = sender;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPerson([FromForm] PersonViewModel person)
        {
            try
            {
                // create via application layer
                var cmd = new CreatePersonCommand(person);
                var created = await _sender.Send(cmd);

                if (Request.IsAjaxRequest())
                    return Json(new { success = true, message = "Person created.", type = "success" });

                return RedirectToAction("People");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = ex.Message, type = "error", details = ex.ToString() });
                }

                return RedirectToAction("People");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePerson([FromForm] Guid id)
        {
            try
            {
                var cmd = new DeletePersonCommand(id);
                var result = await _sender.Send(cmd);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = result, message = "Person removed.", type = "success" });
                }

                return RedirectToAction("People");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = ex.Message, type = "error", details = ex.ToString() });
                }

                return RedirectToAction("People");
            }
        }

        [HttpGet]
        public async Task<IActionResult> People([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            ViewBag.Page = page;
            ViewBag.PageSize = size;
            var persons = await _sender.Send(new GetPersonsQuery(page, size));

            // exclude the currently authenticated user from the list
            var claimVal = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(claimVal, out var uid))
            {
                persons = persons.Where(p => p.Id != uid);
            }

            return View(persons);
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
        public async Task<IActionResult> AddAccount([FromForm] decimal Balance = 0, [FromForm] string AccountType = "")
        {
            try
            {
                // Use current user's NameIdentifier claim as owner
                var claimVal = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(claimVal, out var uid))
                    return Request.IsAjaxRequest()
                        ? Json(new { success = false, message = "Invalid user id", type = "error" })
                        : BadRequest("Invalid user id");

                var cmd = new CreateAccountCommand(uid, Balance, AccountType ?? string.Empty);
                await _sender.Send(cmd);

                if (Request.IsAjaxRequest())
                    return Json(new { success = true, message = "Account created.", type = "success" });

                return RedirectToAction("Accounts");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = ex.Message, type = "error", details = ex.ToString() });

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
                    var updateCmd = new UpdateTransactionCommand(Id.Value, AccountId, Amount, TransactionDate, Description ?? string.Empty);
                    var result = await _sender.Send(updateCmd);
                }
                else
                {
                    var cmd = new CreateTransactionCommand(AccountId, Amount, TransactionDate, Description ?? string.Empty);
                    var result = await _sender.Send(cmd);
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true, message = "Transaction created successfully.", type = "success" });
                }

                return RedirectToAction("Transactions");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = ex.Message, type = "error", details = ex.ToString() });
                }

                return View("Accounts");
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
                        Description = t.Description,
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

                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true, message = "Person updated.", type = "success" });
                }
                return RedirectToAction("People");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = ex.Message, type = "error", details = ex.ToString() });
                }

                return RedirectToAction("People");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccount([FromForm] Guid accountId, [FromForm] bool Close = false)
        {
            try
            {
                var cmd = new UpdateAccountCommand(accountId, Close);
                var result = await _sender.Send(cmd);

                if (Request.IsAjaxRequest()) return Json(new { success = true, message = "Account updated.", type = "success" });

                return RedirectToAction("Accounts");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest()) return Json(new { success = false, message = ex.Message, type = "error", details = ex.ToString() });
                return RedirectToAction("Accounts");
            }
        }
    }
}
