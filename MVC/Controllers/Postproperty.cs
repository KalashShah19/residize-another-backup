using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVC.Controllers
{
    // [Route("[controller]")]
    public class Postproperty : Controller
    {
        private readonly ILogger<Postproperty> _logger;

        public Postproperty(ILogger<Postproperty> logger)
        {
            _logger = logger;
        }

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

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Primarydetails()
        {
            int? UserId = HttpContext.Session.GetInt32("userid");
            string? username = HttpContext.Session.GetString("name");
            if (UserId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Id = UserId;
                ViewBag.username = username;
            }

            return View("Primarydetails");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}