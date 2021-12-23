using System.ComponentModel.DataAnnotations;
namespace Leave_Management_NET5.Models
{
    public class EditLeaveAllocationVM
    {
        public int Id { get; set; }

        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }

        [Display(Name = "Number Of Days")]
        [Range(1, 25, ErrorMessage = "Number of day's should be between 1 and 25")]
        public int NumberOfDays { get; set; }

        public LeaveTypeVM LeaveType { get; set; }

        public int leaveTypeId { get; set; }

    }
}
