using System;
using AutoMapper;
using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Data;
using Leave_Management_NET5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Leave_Management_NET5.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveTypesController : Controller
    {
        //private readonly ILeaveTypeRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LeaveTypesController(ILeaveTypeRepository repo, IMapper mapper, IUnitOfWork unitOfWork)
        {
            //_repo = repo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // GET: LeaveTypesController
        public async Task<ActionResult> Index()
        {
            //var leavetypes = (await _repo.findAll()).ToList();
            var leavetypes = (await _unitOfWork.LeaveTypes.findAll()).ToList();
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetypes);
            return View(model);
        }

        // GET: LeaveTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            //if (!await _repo.isExists(id))
            if (!await _unitOfWork.LeaveTypes.isExists(q => q.Id == id))
            {
                return NotFound();
            }

            //var leavetype = await _repo.FindById(id);
            var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            var model = _mapper.Map<LeaveTypeVM>(leavetype);

            return View(model);
        }

        // GET: LeaveTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var leaveType = _mapper.Map<LeaveType>(model);
                leaveType.DateCreated = DateTime.Now;

                //var isSuccess = await _repo.Create(leaveType);
                //if (!isSuccess)
                //{
                //    ModelState.AddModelError(string.Empty, "Something Went Wrong ...");
                //    return View(model);
                //}

                await _unitOfWork.LeaveTypes.Create(leaveType);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something Went Wrong ...");
                return View();
            }
        }

        // GET: LeaveTypesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            // if (!await _repo.isExists(id))
            if (!await _unitOfWork.LeaveTypes.isExists(q => q.Id == id))
            {
                return NotFound();
            }

            //var leaveType = await _repo.FindById(id);
            var leaveType = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);

            return View(model);
        }

        // POST: LeaveTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var leaveType = _mapper.Map<LeaveType>(model);
                //var isSuccess = await _repo.Update(leaveType);
                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Something Went Worng ...");
                //    return View(model);
                //}

                _unitOfWork.LeaveTypes.Update(leaveType);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something Went Worng ...");
                return View(model);
            }
        }


        // POST: LeaveTypesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                //var leavetype = await _repo.FindById(id);
                var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
                if (leavetype == null)
                {
                    return NotFound();
                }

                //var isSuccess = await _repo.Delete(leavetype);
                //if (!isSuccess)
                //{
                //    return View(model);
                //}

                _unitOfWork.LeaveTypes.Delete(leavetype);
                await _unitOfWork.Save();

            }
            catch
            {
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
