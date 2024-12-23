using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Repository.Models;

public class User
{
    public class Get
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string EmailAddress { get; set; }
        public required string ContactNumber { get; set; }
        public required DateTime JoiningDate { get; set; }
        public required string ImagePath { get; set; }
        public required string UserRole { get; set; }

    }

    public class GetContectInfo
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    //  below Anvi code
    public class GetUpdateProfile
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string EmailAddress { get; set; }
        public required string ContactNumber { get; set; }
        public required string ImagePath { get; set; }
        public IFormFile? ImageFile { get; set; }
        public required string? Password { get; set; }
    }

    public class Post
    {
        [Required(ErrorMessage = "First name cannot be empty")]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[A-Z][a-z]*$", ErrorMessage = "The first letter of your first name must be capital and may not contain whitespaces.")]
        public required string FirstName { get; set; }
        [Required(ErrorMessage = "Last name cannot be empty")]
        [RegularExpression(@"^[A-Z][a-z]*$", ErrorMessage = "The first letter of your last name must be capital and may not contain whitespaces..")]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Email cannot be empty")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address.")]
        public required string EmailAddress { get; set; }
        [Required(ErrorMessage = "Contact number cannot be empty")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid contact number.")]
        public required string ContactNumber { get; set; }
        [Required(ErrorMessage = "Password cannot be empty")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long, contain one uppercase letter, one number, and one special character.")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password cannot be empty")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
        public bool AgreeTerms { get; set; }
    }

    public class ChangePassword
    {
        public int Id { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }

    }

    public class Login
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address.")]
        public required string EmailAddress { get; set; }
        [Required(ErrorMessage = "Password cannot be empty")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long, contain one uppercase letter, one number, and one special character.")]
        public required string Password { get; set; }
    }

    public class Forgot
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter a valid email address."
        )]
        public required string EmailAddress { get; set; }
    }
    public class Resetpassword
    {
        public string NewPassword { get; set; }
        public string EmailAddress { get; set; }

    }
}
