using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Repository.Interfaces;
using Repository.Libraries;
using Repository.Models;
using StackExchange.Redis;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountAPIController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly MailerService _mailerService;

        public AccountAPIController(
            IAccountRepository accountRepository,
            MailerService mailerService
        )
        {
            _accountRepository = accountRepository;
            _mailerService = mailerService;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult SendContect(int propertie_id,string userEmail)
        {
            var data = _accountRepository.GetContectInfo(propertie_id);
            
            _mailerService.SendEmail(
                    userEmail,
                    "Property Owner Information...",
                    MailerService.EmailTemplate2(
                        data.UserName,
                        data.Email,
                        data.Phone
                    )
                );
            _mailerService.SendEmail(
                    data.Email,
                    "your contact information send user",
                    MailerService.EmailTemplate2(
                        "",
                        "",
                        ""
                    )
                );
            return Ok("Mail sent successfully...");
        }

        [HttpPost("Login")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromForm] User.Login credentials)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please fill out the form properly.");
                }

                var user = _accountRepository.Login(credentials);
                return Ok(user);
            }
            catch (UserException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("EmailExists")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CheckEmailExistance([FromQuery] string email)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please fill out the form properly.");
                }

                bool exists = _accountRepository.DoesEmailExist(email);
                return Ok(exists);
            }
            catch (UserException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("Register")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Register([FromForm] User.Post userdetails)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please fill out the form properly.");
                }

                _accountRepository.Register(userdetails);
                string token = _accountRepository.SendVerificationToken(userdetails.EmailAddress);
                _mailerService.SendEmail(
                    userdetails.EmailAddress,
                    "Confirm your registration",
                    MailerService.EmailTemplate(
                        _accountRepository.GetUserName(userdetails.EmailAddress),
                        "Confirm your registration",
                        $"http://localhost:5154/Account/VerifyAccount?email={userdetails.EmailAddress}&token={token}",
                        "Verify your account"
                    )
                );
                return Ok(
                    "Your registration is successful, please check your email to confirm your registration!"
                );
            }
            catch (UserException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("VerifyRegistration")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult VerifyRegistration(string email, string token)
        {
            try
            {
                Console.WriteLine("Starting VerifyRegistration...");
                Console.WriteLine($"Email: {email}, Token: {token}");

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is invalid.");
                    return BadRequest("Please fill out the form properly.");
                }

                if (_accountRepository.DoesTokenExist(email, token))
                {
                    Console.WriteLine("Token exists. Verifying user...");
                    _accountRepository.VerifyUser(email);

                    // Get user details
                    User.Get? user = _accountRepository.GetUserDetails(email);
                    Console.WriteLine($"User details: {JsonSerializer.Serialize(user)}");

                    // RabbitMQ setup
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(
                        queue: "UserRegistrations",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    // Publish a message with persistent delivery mode
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    var notification = new
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EmailAddress = user.EmailAddress,
                        ImagePath = user.ImagePath,
                        UserRole = user.UserRole,
                        Timestamp = DateTime.UtcNow,
                        IsSeen = false,
                    };

                    Console.WriteLine($"Notification: {JsonSerializer.Serialize(notification)}");

                    var message = JsonSerializer.Serialize(notification);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "UserRegistrations",
                        basicProperties: properties,
                        body: body
                    );

                    Console.WriteLine("Message published to RabbitMQ.");

                    return Ok(true);
                }

                Console.WriteLine("Token does not exist.");
                return BadRequest(false);
            }
            catch (UserException e)
            {
                Console.WriteLine($"UserException: {e.Message}");
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}\nStack Trace: {e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("markAllAsRead")]
        public IActionResult MarkAllAsRead()
        {
            try
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
                    "redis-14974.c305.ap-south-1-1.ec2.redns.redis-cloud.com:14974,password=XfdMLTe9w0uaJ18VRsijz03amtGzpk9Z"
                );
                var db = redis.GetDatabase();

                // Set unseenCount to 0 in Redis
                db.StringSet("unseenCount", "0");

                // Fetch all notifications
                var notifications = db.ListRange("notifications");

                // Use a dictionary to track unique notifications by ID
                var uniqueNotifications = new Dictionary<int, Notification>();

                foreach (var notificationJson in notifications)
                {
                    var notification = JsonSerializer.Deserialize<Notification>(notificationJson);
                    if (notification != null)
                    {
                        // Use notification ID as the unique key
                        if (!uniqueNotifications.ContainsKey(notification.Id))
                        {
                            uniqueNotifications[notification.Id] = notification;
                        }

                        // Update Seen status if not already true
                        if (!uniqueNotifications[notification.Id].Seen)
                        {
                            uniqueNotifications[notification.Id].Seen = true;
                        }
                    }
                }

                // Clear Redis key and push only unique, updated notifications
                db.KeyDelete("notifications");
                foreach (var notification in uniqueNotifications.Values)
                {
                    db.ListRightPush("notifications", JsonSerializer.Serialize(notification));
                }

                Console.WriteLine($"Processed {uniqueNotifications.Count} unique notifications.");
                return Ok(new { success = true, message = "All notifications marked as read" });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { success = false, message = ex.Message }
                );
            }
        }

        // below is  anvi code

        [HttpGet]
        [Route("[action]")]
        public IActionResult getUserProfileById(int id)
        {
            var userData = _accountRepository.GetProfile(id);
            return Ok(userData);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateProfile([FromForm] User.GetUpdateProfile getProfile)
        {
            _accountRepository.UpdateProfile(getProfile);
            return Ok("Update Profile Successfully...");
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult ChangePassword([FromForm] User.ChangePassword User)
        {
            if (User == null)
            {
                return BadRequest("Invalid data provided.");
            }
            var valid = _accountRepository.ChangePassword(User);

            if (valid)
            {
                return Ok(new { message = "Password changed successfully." });
            }
            else
            {
                return BadRequest(new { message = "Incorrect old password." });
            }
        }

        [HttpPost("Forgotpassword")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Forgotpassword([FromForm] User.Forgot Useremail)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please fill out the form properly.");
                }

                _accountRepository.Forgotpassword(Useremail);
                string token = _accountRepository.SendVerificationToken(Useremail.EmailAddress);

                _mailerService.SendEmail(
                    Useremail.EmailAddress,
                    "Please reset your password",
                    MailerService.EmailTemplate(
                        _accountRepository.GetUserName(Useremail.EmailAddress),
                        @"We received a request to reset the password for your account. If you made this request, click the button below
                        If you didn’t request a password reset, you can safely ignore this email. Your account remains secure.
                        If you have any questions or need further assistance, feel free to contact us.",
                        $"http://localhost:5154/Account/VerifyForgotpassword?email={Useremail.EmailAddress}&token={token}",
                        "Reset Password"
                    )
                );
                return Ok("please check your email to reset your password");
            }
            catch (UserException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("ResetPassword")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ResetPassword([FromForm] User.Resetpassword Userpassword)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please fill out the form properly.");
                }

                var result = _accountRepository.Resetpassword(Userpassword);
                if (result)
                {
                    return Ok("Password reset successfully!");
                }
                else
                {
                    return BadRequest("Password reset failed. Please try again.");
                }
            }
            catch (UserException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("VerifyForgotpassword")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult VerifyForgotpassword(string email, string token)
        {
            try
            {
                Console.WriteLine("Starting VerifyRegistration...");
                Console.WriteLine($"Email: {email}, Token: {token}");

                if (!ModelState.IsValid)
                {
                    return BadRequest("Please fill out the form properly.");
                }

                if (_accountRepository.DoesTokenExist(email, token))
                {
                    _accountRepository.VerifyUser(email);

                    return Ok(true);
                }

                return BadRequest(false);
            }
            catch (UserException e)
            {
                Console.WriteLine($"UserException: {e.Message}");
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}\nStack Trace: {e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
