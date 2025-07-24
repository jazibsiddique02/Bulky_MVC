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
    [Authorize(Roles = StaticDetails.Role_Admin)]
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
                productVM.Product = _unitOfWork.Product.Get(x => x.Id == id,includeProperies:"ProductImages"); // keep includeProperties name similar to name of Dbset for that model.
                return View(productVM);
            }

        }


        // Single Post method for Create and Update
        // POST: Admin/Products/Upsert
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                // first add product to db so that you can get the id of the added product.
                if (productVM.Product.Id == 0)
                {

                    _unitOfWork.Product.Add(productVM.Product);
                    _unitOfWork.Save();
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                }



                //this will give us the wwwroot folder
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        // sets name of image file with extension
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\product\product-" + productVM.Product.Id; // sets the path of product directory in wwwroot folder. each product will have its imgs in its own folder.
                        // gets the path of product directory in wwwroot folder
                        string finalPath = Path.Combine(wwwRootPath, productPath);


                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath); // creates the product directory if it does not exist
                        }



                        using (var filestream = new FileStream(Path.Combine(finalPath, filename), FileMode.Create))
                        {
                            //Path.Combine(finalPath,filename)    this combines the productPath and filename to create a full path for the file


                            // Copies the uploaded file to product directory of wwwroot folder with new filename
                            file.CopyTo(filestream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + filename, // sets the image URL for the product.eg: \images\product\product-1\filename.jpg
                            ProductId = productVM.Product.Id
                        };


                        // now adding the individual ProductImage obj in List<ProductImage> in Product model

                        // if ProductImages in Product model is null, initialize it
                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImage); // adds the productImage to the ProductImages list of the Product model
                    }

                    _unitOfWork.Product.Update(productVM.Product); // updates the product with the new images
                    _unitOfWork.Save(); // saves the changes to the database                 

                }

                TempData["success"] = "Product created/updated successfully";
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



        public IActionResult DeleteImage(int imageId)
        {
            ProductImage objFromDb = _unitOfWork.ProductImage.Get(x => x.Id == imageId);
            int productId = objFromDb.ProductId; // get the product id from the ProductImage object
            if (objFromDb != null)
            {
                if (!string.IsNullOrEmpty(objFromDb.ImageUrl))
                {
                    var wwwRootPath = _webHostEnvironment.WebRootPath; // gets the wwwroot folder path
                    // delete the image
                    var oldImagePath = Path.Combine(wwwRootPath, objFromDb.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath)) // checks if the  image file exists
                    {
                        System.IO.File.Delete(oldImagePath); // deletes the  image file if it exists
                    }
                }

                _unitOfWork.ProductImage.Remove(objFromDb); // removes the product image from the database
                _unitOfWork.Save(); // saves the changes to the database
                TempData["success"] = "Image deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId }); // redirect to the Upsert action of the same product after deleting its image
        }




     


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
            if (objProduct == null)
            {
                return Json(new { success = false, message = "Error While Deleting." });
            }


            var wwwRootPath = _webHostEnvironment.WebRootPath; // gets the wwwroot folder path
            // delete the image
            //var oldImagePath = Path.Combine(wwwRootPath, objProduct.ImageUrl.TrimStart('\\'));
            //if (System.IO.File.Exists(oldImagePath)) // checks if the  image file exists
            //{
            //    System.IO.File.Delete(oldImagePath); // deletes the  image file if it exists
            //}

            var productPath = @"images\product\product-" + id; // sets the path of product directory in wwwroot folder. each product will have its imgs in its own folder.
            var finalPath = Path.Combine(wwwRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                // before deleting directory, delete all files in it.
                string[] files = Directory.GetFiles(finalPath);
                foreach (string file in files)
                {
                    System.IO.File.Delete(file); // deletes each file in the directory
                }
                // after deleting all files, delete the directory itself
                Directory.Delete(finalPath);
            }

            _unitOfWork.Product.Remove(objProduct); // removes the product from the database
            _unitOfWork.Save(); // saves the changes to the database

            return Json(new { success = true, message = "Product Deleted Successfully." });
        }



        #endregion
    }
}





