using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Repository.Models
{
    public class Project
    {
        public class Post
        {
            [Required(ErrorMessage = "Project Name cannot be empty")]
            [Display(Name = "Project Name")]
            public required string ProjectName { get; set; }

            [Required(ErrorMessage = "Project Details Id cannot be empty")]
            [Display(Name = "Project ID")]
            public required int ProjectDetailsId { get; set; }
            public List<IFormFile> Files { get; set; }
            public List<IFormFile> VideoFiles { get; set; }
            public IFormFile BorchureFile { get; set; }

        }

        public class Update
        {
            [Required(ErrorMessage = "Project Name cannot be empty")]
            [Display(Name = "Project Name")]
            public required string ProjectName { get; set; }

            [Required(ErrorMessage = "Project Details Id cannot be empty")]
            [Display(Name = "Project ID")]
            public required int ProjectDetailsId { get; set; }

            public List<IFormFile>? Files { get; set; }
            public List<IFormFile>? VideoFiles { get; set; }
            public IFormFile? BorchureFile { get; set; }

        }

        public class Assets
        {
            public List<IFormFile>? Files { get; set; }
            public List<IFormFile>? VideoFiles { get; set; }
            public IFormFile? BorchureFile { get; set; }

        }
        public class GET
        {
            public int? ProjectId {get;set;}
            public string ProjectName { get; set; }
            public string PicturePath { get; set; }
            public string BrochurePath { get; set; }
            public bool IsReraVerified { get; set; }
            public int PropertyDetailId { get; set; }
            public string VideoPath { get; set; }
            public int PropertyAge { get; set; }
            public bool AvailabilityStatus { get; set; }
            public string City { get; set; }
            public string Locality { get; set; }
            public string Address { get; set; }
            public int PinCode { get; set; }
            public DateTime PostedDate { get; set; }
            public int? UserId { get; set; }
            public List<string> PictureFiles { get; set; } = new List<string>();
            public List<string> BrochureFiles { get; set; } = new List<string>();
            public List<string> VideoFiles { get; set; } = new List<string>();
        }

        public class Amenities
        {
            public List<int> AllAmenities { get; set; }
            public required int ProjectDetailsId { get; set; }
        }

    }
}