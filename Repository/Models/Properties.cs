using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Repository.Models
{
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
            public  List<string> PropertyPictures { get; set; }
            public  string VideoPath{get;set;}
            public required string Furnished { get; set; }
            public required string PropertyType { get; set; }
            public required string AreaType { get; set; }
            public int PropertyDetailId { get; set; }
            public bool? Sold { get; set; }
        }

        public class Post
        {
            public int? ProjectId { get; set; } = null;
            public required string PropertyName { get; set; }
            public required int PropertyAmount { get; set; }
            public required float PropertySize { get; set; }
            public required string PropertyListingType { get; set; }
            public required List<IFormFile> PropertyPictures { get; set; }
            public required IFormFile PropertyVideos { get; set; }
            public int Bedrooms { get; set; }
            public int Bathrooms { get; set; }
            public int Floor { get; set; }
            public required string Furnished { get; set; }
            public bool TaxInclusion { get; set; }
            public required string PropertyType { get; set; }
            public int PropertyDetailId { get; set; }
            public required string AreaType { get; set; }
        }

        public class UpdateDetails
        {
            public int PropertyId { get; set; }
            public required string PropertyName { get; set; }
            public required int PropertyAmount { get; set; }
            public required float PropertySize { get; set; }
            public required List<IFormFile> PropertyPictures { get; set; }
            public required IFormFile PropertyVideos { get; set; }
            public int Bedrooms { get; set; }
            public int Bathrooms { get; set; }
            public int Floor { get; set; }
            public required string Furnished { get; set; }
            public bool TaxInclusion { get; set; }
        }

        public class PropertyDetails
        {
            public class Post
            {
                public int PropertyAge { get; set; } // in years
                public required string City { get; set; }
                public required string Locality { get; set; }
                public bool ReadytoMove { get; set; }
                public string Address { get; set; } = "";
                public int Pincode { get; set; }
                public int UserId { get; set; }
            }



            public class Update
            {
                public int PropertyAge { get; set; }
                public bool ReadytoMove { get; set; }
                public required string City { get; set; }
                public required string Locality { get; set; }
                public string Address { get; set; } = "";
                public int Pincode { get; set; }
            }
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
        }
    }
}