using Leave_Management_NET5.Data;
using System;
using System.Collections.Generic;
namespace Leave_Management_NET5.Contracts
{
    public interface ILeaveTypeRepository : IRepositoryBase<LeaveType>
    {
        ICollection<LeaveType> GetEmployeesByLeaveType(int id);

        List<LeaveType> GetLeaveTypesByEmployee(string employeeId);
    }

}
