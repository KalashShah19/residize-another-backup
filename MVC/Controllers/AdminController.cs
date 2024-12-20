using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    //[Route("[controller]")]
    public class AdminController : Controller
    {
        public AdminController()
        {

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

        public IActionResult Dashboard()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (IsLogin())
            {
                if (string.Equals("client", GetRole()))
                    return RedirectToAction("Home", "Client");
            }
            int? UserId = HttpContext.Session.GetInt32("userid");
            if (UserId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Id = UserId;
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}