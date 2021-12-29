using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Leave_Management_NET5.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(LeaveRequest entity)
        {
            await _db.LeaveRequests.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
            _db.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveRequest>> findAll()
        {
            return await _db.LeaveRequests
                        .Include(q => q.RequestingEmployee)
                        .Include(q => q.ApprovedBy)
                        .Include(q => q.LeaveType)
                        .ToListAsync();
        }

        public async Task<LeaveRequest> FindById(int id)
        {
            return await _db.LeaveRequests
                        .Include(q => q.RequestingEmployee)
                        .Include(q => q.ApprovedBy)
                        .Include(q => q.LeaveType)
                        .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployee(string employeeId)
        {
            return (await findAll()).Where(q => q.RequestingEmployeeId == employeeId).ToList();
        }

        public async Task<bool> isExists(int id)
        {
            var exists = await _db.LeaveRequests.AnyAsync(q => q.Id == id);
            return exists;
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return await Save();
        }
    }
}
