using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bulky.DataAccess.Data;
using Bulky.Models;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Bulky.Utility;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Customer/Company
        public IActionResult Index()
        {
            return View(_unitOfWork.Company.GetAll());
        }

        

        // GET: Customer/Company/Upsert
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if(id == null)
            {
                return View(company);
            }
            
            else
            {
                company = _unitOfWork.Company.Get(x => x.Id == id);
                return View(company);
            }
        }

        // POST: Customer/Company/Upsert
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    _unitOfWork.Save();
                    TempData["success"] = "Company Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    _unitOfWork.Save();
                    TempData["success"] = "Company Updated successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(company);
        }

      
       



        #region APICALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = companies });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Company company = _unitOfWork.Company.Get(u => u.Id == id);
            if(company == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company Deleted successfully"});
        }



        #endregion
    }
}
