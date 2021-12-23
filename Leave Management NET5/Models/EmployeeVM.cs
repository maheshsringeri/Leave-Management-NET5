using System;
using System.ComponentModel.DataAnnotations;

namespace Leave_Management_NET5.Models
{
    public class EmployeeVM
    {
        public string Id { get; set; }

        [Display(Name = "Uaer Name")]
        public string UserName { get; set; }

        [Display(Name = "Email Id")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Display(Name = "Tax Id")]
        public string TaxId { get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Date Of Joined")]
        public DateTime DateJoined { get; set; }
    }
}
