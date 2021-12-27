using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leave_Management_NET5.Models
{
    public class CreateLeaveRequestVM
    {
        public int Id { get; set; }

        [Display(Name = "Start Date")]
        [Required]
        public string StartDate { get; set; }
        [Display(Name = "End Date")]
        [Required]
        public string EndDate { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }


        [Display(Name = "Comments")]
        [MaxLength(300, ErrorMessage = "comment should not exeded by 300 characters")]
        public string RequestComments { get; set; }
    }

}
