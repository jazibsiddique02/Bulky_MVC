using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Bulky.Utility;

namespace BulkyWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {

        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // to retrieve id of logged in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if(userId != null)
            {
                if(HttpContext.Session.GetInt32(StaticDetails.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                        _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId.Value).Count());
                }

                return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }


        }
    }
}
