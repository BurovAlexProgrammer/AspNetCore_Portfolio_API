using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebDAL.Entities;
using WebDAL.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Login([FromBody] AccountLoginModel model)
        {
            //TODO separate identity to email/phone
            var existAccount = GetIdentifiedAccount(model);

            if (existAccount == null)
            {
                var message = $"Аккаунт с email '{model.AccountIdentity}' не найден.";
                return NotFound(message);
            }

            if (existAccount.password != model.Password)
            {
                var message = "Неверный пароль.";
                return StatusCode(StatusCodes.Status406NotAcceptable, message);
            }

            UpdateAccountToken(existAccount);
            return new ObjectResult(existAccount);
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody] object jsonGuest)
        {
            var guestInput = JsonSerializer.Deserialize<Account>(jsonGuest.ToString()!);

            if (IsCorrectToken(guestInput))
            {
                return new OkResult();
            }

            return new UnauthorizedResult();
        }

        [NonAction]
        private Account GetIdentifiedAccount(AccountLoginModel model)
        {
            using (var db = new AppDbContext())
            {
                var accounts = db.Accounts.Where(x => x.email == model.AccountIdentity).ToList();

                if (accounts.Count > 1)
                {
                    _logger.LogError($"db.Guest - exc: duplicate account email '{model.AccountIdentity}'");
                    return null;
                }

                if (accounts.Count == 0)
                {
                    _logger.LogError($"db.Guest - account email '{model.AccountIdentity}' not found.");
                    return null;
                }

                return accounts.First();
            }
        }

        [NonAction]
        private string UpdateAccountToken(Account account)
        {
            using (var db = new AppDbContext())
            {
                account.token = Guid.NewGuid().ToString();
                db.Update(account);
                db.SaveChanges();
            }

            return account.token;
        }

        [NonAction]
        private bool IsCorrectToken(Account account)
        {
            using (var db = new AppDbContext())
            {
                var accounts = db.Accounts.ToArray();
                return accounts.Any(x => x.token == account.token && x.name == account.name);
                return db.Accounts.Any(x => x.token == account.token && x.name == account.name);
            }
        }
    }
}