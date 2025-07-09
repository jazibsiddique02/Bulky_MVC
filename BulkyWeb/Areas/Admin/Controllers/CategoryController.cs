
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork categoryRepo)
        {
            _unitOfWork = categoryRepo;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork._categoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Display Order cannot exactly match with the Name.");
            }


            if (ModelState.IsValid)
            {
                _unitOfWork._categoryRepository.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Category");
            }

            return View();
        }


        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            Category categoryFromDb = _unitOfWork._categoryRepository.Get(u=> u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }



        [HttpPost]
        public IActionResult Edit(Category obj)
        {



            if (ModelState.IsValid)
            {
                _unitOfWork._categoryRepository.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index", "Category");
            }

            return View();
        }


        public IActionResult Delete(int id)
        {

            if (id == 0)
            {
                return NotFound();
            }

            Category categoryFromDb = _unitOfWork._categoryRepository.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }




        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            Category? obj = _unitOfWork._categoryRepository.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork._categoryRepository.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index", "Category");


        }
    }
}
