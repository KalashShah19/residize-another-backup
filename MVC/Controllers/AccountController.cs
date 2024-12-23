using Microsoft.AspNetCore.Mvc;
using Repository.Models;

namespace MVC.Controllers;

public class AccountController : Controller
{
    const string API_URL = "http://localhost:5293/api";

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

    public IActionResult Login()
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

    [HttpPost]
    public async Task<IActionResult> Login(User.Login credentials)
    {
        if (ModelState.IsValid)
        {
            HttpClient httpClient = new();
            MultipartFormDataContent FormData =
                new()
                {
                    { new StringContent(credentials.EmailAddress), "EmailAddress" },
                    { new StringContent(credentials.Password), "Password" },
                };
            using HttpResponseMessage response = await httpClient.PostAsync(
                $"{API_URL}/AccountAPI/Login",
                FormData
            );
            if (response.IsSuccessStatusCode)
            {
                User.Get? user = await response.Content.ReadFromJsonAsync<User.Get>();
                if (user == null)
                {
                    TempData["message"] = await response.Content.ReadAsStringAsync();
                    return View();
                }

                HttpContext.Session.SetInt32("userid", user.Id);
                HttpContext.Session.SetString("name", user.FirstName);
                HttpContext.Session.SetString("email", user.EmailAddress);
                HttpContext.Session.SetString("contact", user.ContactNumber);
                HttpContext.Session.SetString("role", user.UserRole);
                return string.Equals("admin", user.UserRole)
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Home", "Client");
            }

            TempData["message"] = await response.Content.ReadAsStringAsync();
            return View();
        }
        return View();
    }

    public IActionResult UserRegister()
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

    public IActionResult ForgotPassword()
    {
        var email = TempData["Email"] as string;
        ViewBag.Email = email;
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }
        if (IsLogin())
        {
            if (string.Equals("admin", GetRole()))
                return RedirectToAction("Dashboard", "Admin");
            if (string.Equals("client", GetRole()))
                return RedirectToAction("Home", "Client");
        }

        TempData.Remove("Email");

        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UserRegister(User.Post userdetails)
    {
        if (ModelState.IsValid)
        {
            if (!userdetails.AgreeTerms)
            {
                ModelState.AddModelError(
                    "AgreeTerms",
                    "You must agree to our terms and conditions"
                );
                return View();
            }
            using HttpClient httpClient = new();

            MultipartFormDataContent formData =
                new()
                {
                    { new StringContent(userdetails.FirstName), "FirstName" },
                    { new StringContent(userdetails.LastName), "LastName" },
                    { new StringContent(userdetails.ContactNumber), "ContactNumber" },
                    { new StringContent(userdetails.EmailAddress), "EmailAddress" },
                    { new StringContent(userdetails.Password), "Password" },
                    { new StringContent(userdetails.ConfirmPassword), "ConfirmPassword" },
                };

            HttpResponseMessage response = await httpClient.PostAsync(
                $"{API_URL}/AccountAPI/Register",
                formData
            );

            if (response.IsSuccessStatusCode)
            {
                TempData["message_type"] = "info";
                TempData["message"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("UserRegister");
            }

            TempData["message_type"] = "danger";
            TempData["message"] = await response.Content.ReadAsStringAsync();
        }
        return View();
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        return Json(new { success = true, message = "Logout successful." });
    }

    [HttpGet]
    public async Task<IActionResult> VerifyAccountAsync(string email, string token)
    {
        using HttpClient httpClient = new();
        HttpResponseMessage response = await httpClient.GetAsync(
            $"{API_URL}/AccountAPI/VerifyRegistration?email={email}&token={token}"
        );

        if (response.IsSuccessStatusCode)
        {
            TempData["message_type"] = "info";
            TempData["message"] = "Your account is verified, you can log into your account !";
            return RedirectToAction("Login");
        }
        else
        {
            TempData["message_type"] = "danger";
            TempData["message"] = await response.Content.ReadAsStringAsync();
            return RedirectToAction("UserRegister");
        }
    }

    [HttpGet]
    public async Task<IActionResult> VerifyForgotpasswordAsync(string email, string token)
    {
        using HttpClient httpClient = new();
        HttpResponseMessage response = await httpClient.GetAsync(
            $"{API_URL}/AccountAPI/VerifyForgotpassword?email={email}&token={token}"
        );

        if (response.IsSuccessStatusCode)
        {
            TempData["Email"] = email;
            return RedirectToAction("ForgotPassword", "Account");
        }
        else
        {
            TempData["ErrorMessage"] =
                "Your Verification link is expired ! Please Generate New One";
            HttpContext.Session.Clear();
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            return RedirectToAction("Login");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Forgotpassword(User.Forgot Useremail)
    {
        if (ModelState.IsValid)
        {
            try
            {
                using HttpClient httpClient = new();
                MultipartFormDataContent formData = new();
                formData.Add(new StringContent(Useremail.EmailAddress), "EmailAddress");

                using HttpResponseMessage response = await httpClient.PostAsync(
                    $"{API_URL}/AccountAPI/Forgotpassword",
                    formData
                );

                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, message });
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("fail");
                    return Json(new { success = false, message });
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred: {ex.Message}";
                return Json(new { success = false, message = "errorMessage" });
            }
        }

        return null;
        // return Json(new { success = false, message = "Invalid data" });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(User.Resetpassword userPassword)
    {
        if (ModelState.IsValid)
        {
            try
            {
                using HttpClient httpClient = new();

                MultipartFormDataContent formData =
                    new()
                    {
                        { new StringContent(userPassword.EmailAddress), "EmailAddress" },
                        { new StringContent(userPassword.NewPassword), "NewPassword" },
                    };

                HttpResponseMessage response = await httpClient.PostAsync(
                    $"{API_URL}/AccountAPI/ResetPassword",
                    formData
                );

                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, message });
                }

                string error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
        else
        {
            return Json(new { success = false, message = "Invalid input" });
        }
    }


    public IActionResult GetUserDetail()
    {
        string? email = HttpContext.Session.GetString("email");
        string? username = HttpContext.Session.GetString("name");
        string? contact = HttpContext.Session.GetString("contact");
        return Ok(new User.GetContectInfo() { Email = email, UserName = username, Phone = contact });
    }
}
