using System;
using System.ComponentModel.DataAnnotations;
namespace Leave_Management_NET5.Models
{
    public class LeaveTypeVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Default Number Of Days")]
        [Range(1, 25, ErrorMessage = "Please enter valid days between 1 to 25")]
        public int DefaultDays { get; set; }

        [Display(Name = "Date Created")]
        public DateTime? DateCreated { get; set; }
    }

}
