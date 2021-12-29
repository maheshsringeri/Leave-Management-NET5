using Leave_Management_NET5.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leave_Management_NET5.Contracts
{
    public interface ILeaveRequestRepository : IRepositoryBase<LeaveRequest>
    {
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployee(string employeeId);
    }
}
