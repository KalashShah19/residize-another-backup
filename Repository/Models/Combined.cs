using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Repository.Models
{
    public class Combined
    {

        public class PropertyDetails
        {
            [Required(ErrorMessage = "Project Name cannot be empty")]
            [Display(Name = "Project Name")]
            [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Project Name must contain only alphabetic characters and spaces.")]

            public string ProjectName { get; set; }

            [Required(ErrorMessage = "Please upload at least one file.")]
            [Display(Name = "Files")]
            public List<IFormFile> Files { get; set; }

            [Required(ErrorMessage = "Please upload at least one video.")]
            [Display(Name = "Video Files")]
            public List<IFormFile> VideoFiles { get; set; }

            [Required(ErrorMessage = "Please upload a brochure.")]
            [Display(Name = "Brochure File")]
            public IFormFile BorchureFile { get; set; }

            // Property Details
            [Range(0, int.MaxValue, ErrorMessage = "Property Age must be 0 or greater.")]
            [Display(Name = "Property Age")]
            public int PropertyAge { get; set; }

            [Display(Name = "Ready to Move")]
            public bool ReadytoMove { get; set; }

            [Required(ErrorMessage = "City cannot be empty")]
            [Display(Name = "City")]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must contain only letters and spaces.")]
            public string City { get; set; }

            [Required(ErrorMessage = "Locality cannot be empty")]
            [Display(Name = "Locality")]
            public string Locality { get; set; }

            [Required(ErrorMessage = "Address cannot be empty")]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required(ErrorMessage = "Pincode cannot be empty")]
            [Display(Name = "Pincode")]
            [Range(100000, 999999, ErrorMessage = "Pincode must be a valid 6-digit number.")]
            [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits.")]
            public int Pincode { get; set; }

            [Required(ErrorMessage = "User ID cannot be empty")]
            [Display(Name = "User ID")]
            public int UserId { get; set; }

            public List<int> AllAmenities { get; set; }

        }



    }
}