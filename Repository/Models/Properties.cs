using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Repository.Models;

public class Properties
{
    public class ListGet
    {
        public int PropertyId { get; set; }
        public int? ProjectId { get; set; } = null;
        public required string PropertyName { get; set; }
        public string? ProjectName { get; set; }
        public required int PropertyAmount { get; set; }
        public required int PropertySize { get; set; }
        public int Bedrooms { get; set; }
        public required string PropertyListingType { get; set; }
        public required string PropertyPicture { get; set; }
        public required string Furnished { get; set; }
        public required string PropertyType { get; set; }
        public required string AreaType { get; set; }
        public int PropertyAge { get; set; } // in years
        public required string City { get; set; }
        public required string Locality { get; set; }
        public bool ReadytoMove { get; set; }
        public string Address { get; set; } = "";
    }
    public class Get
    {

        public int PropertyId { get; set; }
        public int? ProjectId { get; set; } = null;
        public required string PropertyName { get; set; }
        public string? ProjectName { get; set; }
        public required int PropertyAmount { get; set; }
        public bool InclusiveTax { get; set; }
        public required int PropertySize { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Floor { get; set; }
        public required string PropertyListingType { get; set; }
        public List<string> PropertyPictures { get; set; }
        public string VideoPath { get; set; }
        public required string Furnished { get; set; }
        public required string PropertyType { get; set; }
        public required string AreaType { get; set; }
        public int PropertyDetailId { get; set; }
        public bool? Sold { get; set; }
    }

    public class Post
    {
        public int? ProjectId { get; set; } = null;
        [Required(ErrorMessage = "Property Name cannot be empty")]
        [Display(Name = "Property Name")]
        public required string PropertyName { get; set; }
        [Required(ErrorMessage = "Please specify the property price.")]
        [Range(9000, int.MaxValue, ErrorMessage = "Price must start from 9 thousand.")]
        public required int PropertyAmount { get; set; }
        [Required(ErrorMessage = "Please specify the number of floors.")]
        [Range(400, 10000, ErrorMessage = "Square feet must be between 400 and 10000.")]
        public required float PropertySize { get; set; }
        public required string PropertyListingType { get; set; }
        [Required(ErrorMessage = "Please upload at least one picture.")]
        [Display(Name = "Pictures")]
        public required List<IFormFile> PropertyPictures { get; set; }
        public IFormFile? PropertyVideos { get; set; } = null;
        [Required(ErrorMessage = "Please specify the number of bedrooms.")]
        [Range(1, 5, ErrorMessage = "Number of bedrooms must be between 1 and 5.")]
        public int Bedrooms { get; set; }
        [Required(ErrorMessage = "Please specify the number of bathrooms.")]
        [Range(1, 5, ErrorMessage = "Number of bathrooms must be between 1 and 5.")]
        public int Bathrooms { get; set; }
        [Required(ErrorMessage = "Please specify the number of floors.")]
        [Range(0, 15, ErrorMessage = "Number of floors must be between 0 and 15.")]
        public int Floor { get; set; }
        [Required(ErrorMessage = "Please specify furnishing type.")]
        public required string Furnished { get; set; }
        public bool TaxInclusion { get; set; }
        [Required(ErrorMessage = "Please specify property type.")]
        public required string PropertyType { get; set; }
        public int PropertyDetailId { get; set; } //
        [Required(ErrorMessage = "Please specify area type.")]
        public required string AreaType { get; set; }
    }

    public class UpdateDetails
    {
        public int PropertyId { get; set; }
        [Required(ErrorMessage = "Property Name cannot be empty")]
        [Display(Name = "Property Name")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Project Name must contain only alphabetic characters and spaces.")]
        public required string PropertyName { get; set; }
        [Required(ErrorMessage = "Please specify the property price.")]
        [Range(1200000, int.MaxValue, ErrorMessage = "Price must start from 12 lakhs.")]
        public required int PropertyAmount { get; set; }
        [Required(ErrorMessage = "Please specify the number of floors.")]
        [Range(400, 10000, ErrorMessage = "Square feet must be between 400 and 10000.")]
        public required float PropertySize { get; set; }
        public List<IFormFile>? PropertyPictures { get; set; }
        public IFormFile? PropertyVideos { get; set; }
        [Required(ErrorMessage = "Please specify the number of bedrooms.")]
        [Range(1, 5, ErrorMessage = "Number of bedrooms must be between 1 and 5.")]
        public int Bedrooms { get; set; }
        [Required(ErrorMessage = "Please specify the number of bathrooms.")]
        [Range(1, 5, ErrorMessage = "Number of bathrooms must be between 1 and 5.")]
        public int Bathrooms { get; set; }
        [Required(ErrorMessage = "Please specify the number of floors.")]
        [Range(0, 15, ErrorMessage = "Number of floors must be between 0 and 15.")]
        public int Floor { get; set; }
        [Required(ErrorMessage = "Please specify furnishing type.")]
        public required string Furnished { get; set; }
        public bool TaxInclusion { get; set; }
    }

    public class PropertyDetails
    {
        public class Get()
        {
            public int DetailId { get; set; }
            public int PropertyAge { get; set; } // in years
            public required string City { get; set; }
            public required string Locality { get; set; }
            public bool ReadytoMove { get; set; }
            public int Pincode { get; set; }
            public string Address { get; set; } = "";
            public required DateTime PostedDate { get; set; }
            public required int UserId { get; set; }
        }

        public class PropertyWithDetails
        {
            public Properties.Get Property { get; set; }
            public PropertyDetails.Get Details { get; set; }
        }
        public class Post
        {
            [Range(0, int.MaxValue, ErrorMessage = "Property Age must be 0 or greater.")]
            [Display(Name = "Property Age")]
            public int PropertyAge { get; set; } // in years
            [Required(ErrorMessage = "City cannot be empty")]
            [Display(Name = "City")]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must contain only letters and spaces.")]
            public required string City { get; set; }
            [Required(ErrorMessage = "Locality cannot be empty")]
            [Display(Name = "Locality")]
            public required string Locality { get; set; }
            [Display(Name = "Ready to Move")]
            public bool ReadytoMove { get; set; }
            public string Address { get; set; } = "";
            [Required(ErrorMessage = "Pincode cannot be empty")]
            [Display(Name = "Pincode")]
            [Range(100000, 999999, ErrorMessage = "Pincode must be a valid 6-digit number.")]
            [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits.")]
            public int Pincode { get; set; }
            [Required(ErrorMessage = "Please specify user id.")]
            public int UserId { get; set; }
        }

        public class Update
        {
            [Range(0, int.MaxValue, ErrorMessage = "Property Age must be 0 or greater.")]
            [Display(Name = "Property Age")]
            public int PropertyAge { get; set; }
            public bool ReadytoMove { get; set; }
            [Required(ErrorMessage = "City cannot be empty")]
            [Display(Name = "City")]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must contain only letters and spaces.")]
            public required string City { get; set; }
            [Required(ErrorMessage = "Locality cannot be empty")]
            [Display(Name = "Locality")]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Locality must contain only letters and spaces.")]
            public required string Locality { get; set; }
            public string Address { get; set; } = "";
            [Required(ErrorMessage = "Pincode cannot be empty")]
            [Display(Name = "Pincode")]
            [Range(100000, 999999, ErrorMessage = "Pincode must be a valid 6-digit number.")]
            [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits.")]
            public int Pincode { get; set; }
        }
    }
}