using Leave_Management_NET5.Models;
using System.Collections.Generic;

namespace Leave_Management_NET5.Models
{
    public class EmployeeLeaveRequestViewVM
    {
        public List<LeaveAllocationVM> leaveAllocations { get; set; }
        public List<LeaveRequestVM> leaveRequests { get; set; }
    }
}
