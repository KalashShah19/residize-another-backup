using System.ComponentModel.DataAnnotations;

namespace Repository.Models
{
    public class Reviews
    {
        public class Get
        {
            public int Rating { get; set; }
            public string Comments { get; set; }
            public string CreatedAt { get; set; }
            [Required]
            public int UserId { get; set; }
            public string ImagePath { get; set; }
            public string Username{get; set; }
        }

        public class Post
        {
            [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
            public int Rating { get; set; }

            [MaxLength(500, ErrorMessage = "Comments cannot exceed 500 characters.")]
            public string Comments { get; set; }

            [Required(ErrorMessage = "UserId is required.")]
            public int UserId { get; set; }
        }
    }
}

