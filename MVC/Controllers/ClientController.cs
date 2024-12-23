using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVC.Controllers
{
    //[Route("[controller]")]
    public class ClientController : Controller
    {

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

        public ClientController()
        {

        }
        public IActionResult MyInterest()
        {
            int? UserId = HttpContext.Session.GetInt32("userid");
            if (UserId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Id = UserId;
            return View();
        }

        public IActionResult Home()
        {
            // Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            // Response.Headers["Pragma"] = "no-cache";
            // Response.Headers["Expires"] = "0";

            // if (IsLogin())
            // {
            //     if (string.Equals("admin", GetRole()))
            //         return RedirectToAction("Dashboard", "Admin");
            // }

            // int? UserId = HttpContext.Session.GetInt32("userid");
            // if (UserId == null)
            // {
            //     return RedirectToAction("Login", "Account");
            // }
            // ViewBag.Id = UserId;
            return View();
        }
        public IActionResult User()
        {
            if (IsLogin())
            {
                if (string.Equals("admin", GetRole()))
                    return RedirectToAction("Dashboard", "Admin");
                if (string.Equals("client", GetRole()))
                    return RedirectToAction("Home", "Client");
            }


            return View();
        }

        public IActionResult ViewProject()
        {
            // if (HttpContext.Session.GetString("email") == null)
            // {
            //     return RedirectToAction("User", "Client");
            // }
            // ViewBag.loginUserEmail = HttpContext.Session.GetString("email");
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        public IActionResult ViewProperty()
        {
            return View();
        }

        public IActionResult ViewOneProperty(int id)
        {
            ViewBag.PropId = id;
            return View();
        }
        public IActionResult ViewOnePropert(int id)
        {
            ViewBag.PropId = id;
            return View();
        }


        public IActionResult ViewOneProject(int id)
        {
            ViewBag.ProjId = id;
            Console.WriteLine(id);
            return View(id);
        }
    }
}