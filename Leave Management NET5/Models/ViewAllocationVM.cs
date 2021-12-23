using System.Collections.Generic;

namespace Leave_Management_NET5.Models
{
    public class ViewAllocationVM
    {
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }

        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
    }
}
