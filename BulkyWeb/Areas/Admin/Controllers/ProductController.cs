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
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Bulky.Utility;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =StaticDetails.Role_Admin)]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;


        // Injecting IWebHostEnvironment to access the web host environment
        //This helps us in accessing wwwroot folder for storing images
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork productRepo, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = productRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Products
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperies: "Category").ToList();

            return View(objProductList);
        }








        // GET: Admin/Products/Upsert
        //Combining Create and Update Pages
        public IActionResult Upsert(int? id)  // Update + Insert
        {

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
               )
            };

            if (id == null || id == 0)
            {
                // Create
                return View(productVM);
            }
            else
            {
                // Update
                productVM.Product = _unitOfWork.Product.Get(x => x.Id == id);
                return View(productVM);
            }

        }


        // Single Post method for Create and Update
        // POST: Admin/Products/Upsert
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                //this will give us the wwwroot folder
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    //sets name of image file with extension
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // gets the path of product directory in wwwroot folder
                    string productPath = Path.Combine(wwwRootPath, @"images\product");


                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // delete old image and add new one
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath)) // checks if the old image file exists
                        {
                            System.IO.File.Delete(oldImagePath); // deletes the old image file if it exists
                        }
                    }

                    using (var filestream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        // Path.Combine(productPath,filename)    this combines the productPath and filename to create a full path for the file


                        // Copies the uploaded file to product directory of wwwroot folder with new filename
                        file.CopyTo(filestream);
                    }


                    productVM.Product.ImageUrl = @"\images\product\" + filename; // sets the image URL for the product

                }

                if (productVM.Product.Id == 0)
                {

                    _unitOfWork.Product.Add(productVM.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product updated successfully";
                }

                
                return RedirectToAction(nameof(Index));
            }
            else
            {


                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
               );
                return View(productVM);
            }
        }




        //private bool ProductExists(int id)
        //{
        //    return _unitOfWork.Product.Get(x => x.Id == id) != null;
        //}


        #region API CALLS
        //  Areas/Admin/Product/GetAll
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperies: "Category").ToList();
            return Json(new { data = objProductList });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objProduct = _unitOfWork.Product.Get(x => x.Id == id);
            if(objProduct == null)
            {
                return Json(new { success = false, message = "Error While Deleting."});
            }


            var wwwRootPath = _webHostEnvironment.WebRootPath; // gets the wwwroot folder path
            // delete the image
            var oldImagePath = Path.Combine(wwwRootPath, objProduct.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath)) // checks if the  image file exists
            {
                System.IO.File.Delete(oldImagePath); // deletes the  image file if it exists
            }

            _unitOfWork.Product.Remove(objProduct); // removes the product from the database
            _unitOfWork.Save(); // saves the changes to the database

            return Json(new {success = true, message="Product Deleted Successfully."});
        }



        #endregion
    }
}





