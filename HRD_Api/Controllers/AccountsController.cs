using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using HRD_Api.Data;
using HRD_DataLibrary.Errors;
using HRD_DataLibrary.General;
using HRD_DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using HRD_Api.Extentions;

namespace HRD_Api.Controllers
{
    [Produces("application/json")]
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        private readonly HRD_DbContext _context;

        public AccountsController(HRD_DbContext context)
        {
            _context = context;
        }

        // POST: api/accounts/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Account account)
        {
            if (_context.Accounts.Any(a => a.Login == account.Login))
            {
                string password = _context.Accounts
                    .First(a => a.Login == account.Login)
                    .Password;

                if (password == account.Password)
                    return Json(Session());
                else
                {
                    Response.StatusCode = 412;
                    return Json(ErrorType.WrongPassword);
                }
            }

            Response.StatusCode = 412;
            return Json(ErrorType.NonExistentLogin);
        }

        // POST: api/accounts/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Account account)
        {
            if (!_context.Accounts.Any(a => a.Login == account.Login))
            {
                _context.Accounts.Add(account);
                _context.SaveChanges();
                
                return Json(Session());
            }
            Response.StatusCode = 412;
            return Json(ErrorType.WrongPassword);
        }

        private AuthSession Session()
        {
            AuthSession authSession = AuthSession.GetInstance();
            SessionLogic.Instance.Add(authSession);
            return authSession;
        }
    }
}