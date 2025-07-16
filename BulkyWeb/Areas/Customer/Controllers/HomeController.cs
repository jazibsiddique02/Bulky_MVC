using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperies : "Category");
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart cart = new ()
            {
                Product = _unitOfWork.Product.Get(x => x.Id == id, includeProperies: "Category"),
                ProductId = id,
                Count = 1

            };
            
            return View(cart);
        }


        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            // to retrieve id of logged in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            // if 1 user adds the same product to cart again
            var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.ApplicationUserId == userId &&
            x.ProductId == cart.ProductId);
            if(cartFromDb != null)
            {
                // a single user is trying to add the same product to cart.instead of adding another entry, we will just update the count.
                cartFromDb.Count = cartFromDb.Count + cart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                // a user is adding a product to cart for the first time
                _unitOfWork.ShoppingCart.Add(cart);
            }

            _unitOfWork.Save();
            TempData["success"] = "Added to Cart Successfully";
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
