using Leave_Management_NET5.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leave_Management_NET5.Contracts
{
    public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        Task<bool> CheckAllocation(int leaveTypeId, string EmployeeId);
        Task<ICollection<LeaveAllocation>> GetLeaveAllocationByEmployee(string employeeId);

        Task<LeaveAllocation> GetLeaveAllocationByEmployeeAndType(string employeeId, int leaveTypeId);


    }
}
