using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Leave_Management_NET5.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period = DateTime.Now.Year;

            return (await findAll())
                    .Where(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId && q.Period == period)
                    .Any();
        }

        public async Task<bool> Create(LeaveAllocation entity)
        {
            await _db.LeaveAllocations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveAllocation>> findAll()
        {
            return await _db.LeaveAllocations.Include(q => q.LeaveType).ToListAsync();
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            return await _db.LeaveAllocations
                        .Include(q => q.Employee)
                        .Include(q => q.LeaveType)
                        .FirstOrDefaultAsync(q => q.Id == id);

        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationByEmployee(string employeeId)
        {
            var period = DateTime.Now.Year;
            return (await findAll()).Where(q => q.EmployeeId == employeeId && q.Period == period).ToList();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationByEmployeeAndType(string employeeId, int leaveTypeId)
        {
            var period = DateTime.Now.Year;
            var leaveAllocation = (await findAll())
                                    .FirstOrDefault(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId && q.Period == period);

            return leaveAllocation;
        }

        public async Task<bool> isExists(int id)
        {
            var exists = await _db.LeaveAllocations.AnyAsync(q => q.Id == id);
            return exists;

        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}
