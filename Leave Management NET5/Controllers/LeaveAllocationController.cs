using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Data;
using Leave_Management_NET5.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Leave_Management_NET5.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        //private readonly ILeaveTypeRepository _leaveTypeRepository;
        //private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly UserManager<Employee> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LeaveAllocationController(ILeaveTypeRepository leaveTypeRepository,
            ILeaveAllocationRepository leaveAllocationRepository,
            UserManager<Employee> userManager, IUnitOfWork unitOfWork, IMapper _mapper)
        {
            //this._leaveTypeRepository = leaveTypeRepository;
            //this._leaveAllocationRepository = leaveAllocationRepository;
            this._userManager = userManager;
            this._mapper = _mapper;
            _unitOfWork = unitOfWork;
        }

        // GET: LeaveAllocationController
        public async Task<ActionResult> Index()
        {
            //var leaveTypes = (await _leaveTypeRepository.findAll()).ToList();
            var leaveTypes = (await _unitOfWork.LeaveTypes.findAll()).ToList();
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes);
            var model = new CreateLeaveAllocationVM
            {
                leaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);
        }

        public async Task<ActionResult> SetLeave(int Id)
        {
            //var leaveType = await _leaveTypeRepository.FindById(Id);
            var leaveType = await _unitOfWork.LeaveTypes.Find(q => q.Id == Id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var period = DateTime.Now.Year;


            foreach (var employee in employees)
            {
                //if (await _leaveAllocationRepository.CheckAllocation(Id, employee.Id))
                if (await _unitOfWork.LeaveAllocations.isExists(q => q.LeaveTypeId == Id && q.EmployeeId == employee.Id && q.Period == period))
                    continue;

                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = employee.Id,
                    LeaveTypeId = Id,
                    NumberOfDays = leaveType.DefaultDays,
                    Period = DateTime.Now.Year
                };

                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
                // await _leaveAllocationRepository.Create(leaveAllocation);
                await _unitOfWork.LeaveAllocations.Create(leaveAllocation);
                await _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> ListEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var model = _mapper.Map<List<EmployeeVM>>(employees);

            return View(model);
        }

        // GET: LeaveAllocationController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var period = DateTime.Now.Year;
            var employee = _mapper.Map<EmployeeVM>(await _userManager.FindByIdAsync(id));
            var leaveAllocation = await _unitOfWork.LeaveAllocations.findAll(q => q.EmployeeId == id && q.Period == period, includes: q => q.Include(x => x.LeaveType));
            var allocations = _mapper.Map<List<LeaveAllocationVM>>(leaveAllocation);

            var model = new ViewAllocationVM
            {
                Employee = employee,
                LeaveAllocations = allocations
            };

            return View(model);
        }

        // GET: LeaveAllocationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocationController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            // var leaveAllocation = await _leaveAllocationRepository.FindById(id);
            var leaveAllocation = await _unitOfWork.LeaveAllocations.Find(q => q.Id == id, includes: q => q.Include(x => x.Employee).Include(x => x.LeaveType));
            var model = _mapper.Map<EditLeaveAllocationVM>(leaveAllocation);

            return View(model);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var employee = await _userManager.FindByIdAsync(model.EmployeeId);
                    // var leaveTyep = await _leaveTypeRepository.FindById(model.leaveTypeId);
                    var leaveTyep = await _unitOfWork.LeaveTypes.Find(q => q.Id == model.leaveTypeId);

                    model.Employee = _mapper.Map<EmployeeVM>(employee);
                    model.EmployeeId = employee.Id;
                    model.LeaveType = _mapper.Map<LeaveTypeVM>(leaveTyep);

                    return View(model);
                }


                //var leaveAllocation = await _leaveAllocationRepository.FindById(model.Id);
                var leaveAllocation = await _unitOfWork.LeaveAllocations.Find(q => q.Id == model.Id);
                leaveAllocation.NumberOfDays = model.NumberOfDays;

                //var isSuccess = await _leaveAllocationRepository.Update(leaveAllocation);

                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Error occurred while saving ..");
                //    return View(model);
                //}

                _unitOfWork.LeaveAllocations.Update(leaveAllocation);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Details), new { id = model.EmployeeId });
            }
            catch
            {
                return View(model);
            }
        }

        // GET: LeaveAllocationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
