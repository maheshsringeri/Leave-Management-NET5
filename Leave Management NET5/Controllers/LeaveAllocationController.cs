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

namespace Leave_Management_NET5.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly UserManager<Employee> _userManager;
        private readonly IMapper _mapper;

        public LeaveAllocationController(ILeaveTypeRepository leaveTypeRepository,
            ILeaveAllocationRepository leaveAllocationRepository,
            UserManager<Employee> userManager,
            IMapper _mapper)
        {
            this._leaveTypeRepository = leaveTypeRepository;
            this._leaveAllocationRepository = leaveAllocationRepository;
            this._userManager = userManager;
            this._mapper = _mapper;
        }

        // GET: LeaveAllocationController
        public ActionResult Index()
        {
            var leaveTypes = _leaveTypeRepository.findAll().ToList();
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes);
            var model = new CreateLeaveAllocationVM
            {
                leaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);
        }

        public ActionResult SetLeave(int Id)
        {
            var leaveType = _leaveTypeRepository.FindById(Id);
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;

            foreach (var employee in employees)
            {
                if (_leaveAllocationRepository.CheckAllocation(Id, employee.Id))
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
                _leaveAllocationRepository.Create(leaveAllocation);

            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult ListEmployees()
        {
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeVM>>(employees);

            return View(model);
        }

        // GET: LeaveAllocationController/Details/5
        public ActionResult Details(string id)
        {
            var employee = _mapper.Map<EmployeeVM>(_userManager.FindByIdAsync(id).Result);
            var allocations = _mapper.Map<List<LeaveAllocationVM>>(_leaveAllocationRepository.GetLeaveAllocationByEmployee(id));

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
        public ActionResult Edit(int id)
        {
            var leaveAllocation = _leaveAllocationRepository.FindById(id);
            var model = _mapper.Map<EditLeaveAllocationVM>(leaveAllocation);

            return View(model);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var employee = _userManager.FindByIdAsync(model.EmployeeId).Result;
                    var leaveTyep = _leaveTypeRepository.FindById(model.leaveTypeId);

                    model.Employee = _mapper.Map<EmployeeVM>(employee);
                    model.EmployeeId = employee.Id;
                    model.LeaveType = _mapper.Map<LeaveTypeVM>(leaveTyep);

                    return View(model);
                }


                var leaveAllocation = _leaveAllocationRepository.FindById(model.Id);
                leaveAllocation.NumberOfDays = model.NumberOfDays;

                var isSuccess = _leaveAllocationRepository.Update(leaveAllocation);

                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Error occurred while saving ..");
                    return View(model);
                }


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
