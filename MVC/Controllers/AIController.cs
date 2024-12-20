using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVC.Controllers
{
    public class AIController : Controller
    {
        private readonly ILogger<AIController> _logger;

        public bool IsLogin()
        {
            int? UserId = HttpContext.Session.GetInt32("userid");
            return UserId.HasValue;
        }

        public string GetRole()
        {
            string? role = HttpContext.Session.GetString("role");
            if (string.IsNullOrEmpty(role))
                throw new Exception("You are not login");
            return role;
        }

        public AIController(ILogger<AIController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!IsLogin())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public IActionResult Price()
        {
            return View();
        }

        public IActionResult Rent()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}