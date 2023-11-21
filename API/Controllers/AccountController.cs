using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;

        public AccountController(ILogger<AccountController> logger, UserManager<Account> userManager, SignInManager<Account> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AccountLoginModel model)
        {
            var existAccount = await _userManager.FindByEmailAsync(model.AccountIdentity);
            //TODO separate identity to email/phone

            if (existAccount == null)
            {
                var message = $"Аккаунт с email '{model.AccountIdentity}' не найден.";
                return NotFound(message);
            }

            var result = await _signInManager.PasswordSignInAsync(existAccount, model.Password, false, false);

            // if (existAccount.password != model.Password)
            // {
            // var message = "Неверный пароль.";
            // return StatusCode(StatusCodes.Status406NotAcceptable, message);
            // }

            // UpdateAccountToken(existAccount);
            // return new ObjectResult(existAccount);

            // if (result.Succeeded)
            // {
            //     return Redirect(model.ReturnUrl);
            // }

            return new ObjectResult(result);
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