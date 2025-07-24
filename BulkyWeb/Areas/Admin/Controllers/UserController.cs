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
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;



        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Customer/Company
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult RoleManagement(string userId)
        {
            RoleManagementVM roleManagementVM = new();

            roleManagementVM.ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId, includeProperies: "Company");


            roleManagementVM.CurrentRole = _userManager.GetRolesAsync(roleManagementVM.ApplicationUser).GetAwaiter().GetResult().FirstOrDefault();


            roleManagementVM.Roles = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });

            roleManagementVM.Companies = _unitOfWork.Company.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            if (roleManagementVM.ApplicationUser.Company != null)
            {
                roleManagementVM.CurrentCompany = roleManagementVM.ApplicationUser.Company.Name;
            }





            return View("RoleManagement", roleManagementVM);
        }



        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {

            if (roleManagementVM.CurrentRole != StaticDetails.Role_Company)
            {
                roleManagementVM.CurrentCompany = null;

            }

            //var oldRoleId = _unitOfWork.RoleManagement.GetAllUserRoles().FirstOrDefault(x => x.UserId == roleManagementVM.ApplicationUser.Id);


            var userFromDb = _unitOfWork.ApplicationUser.GetAll(includeProperies: "Company").FirstOrDefault(x => x.Id == roleManagementVM.ApplicationUser.Id);

            var oldRole = _userManager.GetRolesAsync(userFromDb).GetAwaiter().GetResult().FirstOrDefault();

            if (roleManagementVM.CurrentRole != oldRole)
            {

                if (roleManagementVM.CurrentRole == StaticDetails.Role_Company)
                {
                    userFromDb.CompanyId = int.Parse(roleManagementVM.CurrentCompany);
                }

                if (oldRole == StaticDetails.Role_Company)
                {
                    userFromDb.CompanyId = null;
                }








            }

            else if (roleManagementVM.CurrentRole == oldRole && userFromDb.CompanyId != int.Parse(roleManagementVM.CurrentCompany))
            {

                userFromDb.CompanyId = int.Parse(roleManagementVM.CurrentCompany);

            }
            _unitOfWork.Save();

            _userManager.RemoveFromRoleAsync(userFromDb, oldRole).GetAwaiter().GetResult();

            _userManager.AddToRoleAsync(userFromDb, roleManagementVM.CurrentRole).GetAwaiter().GetResult();

            return RedirectToAction("Index");







        }








        #region APICALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll(null, includeProperies: "Company").ToList();



            foreach (var user in users)
            {

                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new()
                    {
                        Name = "",

                    };
                }
            }
            return Json(new { data = users });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {

            ApplicationUser user = _unitOfWork.ApplicationUser.Get(x => x.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking user" });
            }


            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                // user is locked and we have to unlock the user.
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                // user is not locked and we have to lock the user
                user.LockoutEnd = DateTime.Now.AddYears(1000); // Locking the user for 1000 years.
            }
            _unitOfWork.Save();

            return Json(new { success = true, message = "Lock/Unlock Successful." });
        }



        #endregion
    }
}
