using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management_NET5.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILeaveAllocationRepository leaveAllocationRepository;

        public LeaveTypeRepository(ApplicationDbContext db, ILeaveAllocationRepository leaveAllocationRepository)
        {
            _db = db;
            this.leaveAllocationRepository = leaveAllocationRepository;
        }

        public async Task<bool> Create(LeaveType entity)
        {
            await _db.LeaveTypes.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveType entity)
        {
            _db.LeaveTypes.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveType>> findAll()
        {
            return await _db.LeaveTypes.ToListAsync();
        }

        public async Task<LeaveType> FindById(int id)
        {
            return await _db.LeaveTypes.FindAsync(id);
        }

        public async Task<ICollection<LeaveType>> GetEmployeesByLeaveType(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LeaveType>> GetLeaveTypesByEmployee(string employeeId)
        {
            var leaveAllocations = (await leaveAllocationRepository.findAll()).Where(q => q.EmployeeId == employeeId).ToList();

            var leaveTypes = (await findAll()).Join(leaveAllocations,
                                                LT => LT.Id,
                                                LA => LA.LeaveTypeId,
                                                (LT, LA) => new LeaveType
                                                {
                                                    Id = LT.Id,
                                                    Name = LT.Name,
                                                    DefaultDays = LT.DefaultDays,
                                                    DateCreated = LT.DateCreated
                                                }).Distinct().ToList();

            return leaveTypes;
        }

        public async Task<bool> isExists(int id)
        {
            var exists = await _db.LeaveTypes.AnyAsync(q => q.Id == id);
            return exists;
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> Update(LeaveType entity)
        {
            _db.LeaveTypes.Update(entity);
            return await Save();
        }
    }
}
