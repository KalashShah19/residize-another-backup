using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Notification
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string ImagePath { get; set; }
        public string UserRole { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Seen { get; set; }
    }
}