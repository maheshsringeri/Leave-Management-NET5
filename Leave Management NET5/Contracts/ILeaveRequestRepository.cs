using Leave_Management_NET5.Data;
using System;
using System.Collections.Generic;

namespace Leave_Management_NET5.Contracts
{
    public interface ILeaveRequestRepository : IRepositoryBase<LeaveRequest>
    {
        List<LeaveRequest> GetLeaveRequestsByEmployee(string employeeId);
    }
}
