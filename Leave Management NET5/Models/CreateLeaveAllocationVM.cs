using System.Collections.Generic;

namespace Leave_Management_NET5.Models
{
    public class CreateLeaveAllocationVM
    {
        public int NumberUpdated { get; set; }
        public List<LeaveTypeVM> leaveTypes { get; set; }
    }
}
