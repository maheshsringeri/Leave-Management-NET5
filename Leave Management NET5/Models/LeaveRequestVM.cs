using System;
using System.ComponentModel.DataAnnotations;
using Leave_Management_NET5.Models;

namespace Leave_Management_NET5.Models
{
    public class LeaveRequestVM
    {
        public int Id { get; set; }

        [Display(Name = "Employee Name")]
        public EmployeeVM RequestingEmployee { get; set; }

        public string RequestingEmployeeId { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Leave Type")]
        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime DateRequested { get; set; }
        public DateTime DateActioned { get; set; }
        public bool? Approved { get; set; }
        public EmployeeVM ApprovedBy { get; set; }
        public string ApprovedById { get; set; }

        public bool Cancelled { get; set; }

        [Display(Name = "Comments")]
        public string RequestComments { get; set; }

        [Display(Name = "Number Of Day's Requested")]
        public int? TotalDaysRequested { get; set; }
    }
}
