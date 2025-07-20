using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM orderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Details(int orderId)
        {
            orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperies: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperies: "Product"),
            };

            return View(orderVM);
        }




        // method for payments made by companies
        [ActionName("Details")]
        [HttpPost]
        public IActionResult DetailsPayNow()
        {

            orderVM.OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id, includeProperies: "ApplicationUser");
            orderVM.OrderDetails = _unitOfWork.OrderDetail.GetAll(x=>x.OrderHeaderId == orderVM.OrderHeader.Id,includeProperies:"Product");
            // stripe logic
            var domain = "https://localhost:7132/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };


            // populating LineItems
            foreach (OrderDetail cart in orderVM.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cart.Price * 100),  // 20rs => 2,000paisa. stripe accepts payment in smallest unit of currency.
                        Currency = "pkr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Product.Title
                        }

                    },
                    Quantity = cart.Count
                };
                options.LineItems.Add(sessionLineItem);
            }


            var service = new SessionService();

            // this contains sessionId and paymentIntentId
            Session session = service.Create(options);


            _unitOfWork.OrderHeader.UpdateStripePaymentId(orderVM.OrderHeader.Id, session.PaymentIntentId, session.Id);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        


         public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader objFromDB = _unitOfWork.OrderHeader.Get(x => x.Id == orderHeaderId);
            if (objFromDB.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {

                //retrieving the relevant session from stripe server
                var service = new SessionService();
                Session session = service.Get(objFromDB.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    HttpContext.Session.Clear();
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.PaymentIntentId, session.Id);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, objFromDB.OrderStatus, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

          


            return View(orderHeaderId);
        }








        [Authorize(Roles = StaticDetails.Role_Admin+","+StaticDetails.Role_Employee)]
        [HttpPost]
        public IActionResult UpdateOrderDetail()
        {
            OrderHeader objFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.OrderHeader.Id);

            objFromDb.Name = orderVM.OrderHeader.Name;
            objFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            objFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            objFromDb.State = orderVM.OrderHeader.State;
            objFromDb.PostalCode = orderVM.OrderHeader.PostalCode;
            objFromDb.City = orderVM.OrderHeader.City;

            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                objFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }

            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                objFromDb.Carrier = orderVM.OrderHeader.Carrier;
            }

            _unitOfWork.OrderHeader.Update(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Details Edited Successfully!";
            return RedirectToAction(nameof(Details),new {orderId = objFromDb.Id});

        }



        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        [HttpPost]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, StaticDetails.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated!";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
            
        }




        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader objFromDB = _unitOfWork.OrderHeader.Get(x=>x.Id == orderVM.OrderHeader.Id);

            objFromDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            objFromDB.ShippingDate = DateTime.Now;
            objFromDB.OrderStatus = StaticDetails.StatusShipped;
            objFromDB.Carrier = orderVM.OrderHeader.Carrier;
            if(objFromDB.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                objFromDB.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(objFromDB);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Updated!";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });

        }






        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader objFromDB = _unitOfWork.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id);

           
            if (objFromDB.PaymentStatus == StaticDetails.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = objFromDB.PaymentIntentId,
                };

                var service = new RefundService();
                service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(objFromDB.Id, StaticDetails.StatusCancelled,StaticDetails.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(objFromDB.Id, StaticDetails.StatusCancelled,StaticDetails.StatusCancelled);

            }

            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully!";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });

        }




        #region API CALLS
        //  Areas/Admin/Order/GetAll
        [HttpGet]
        public IActionResult GetAll(string status)
        {

            IEnumerable<OrderHeader> orderHeaders;

            if(User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperies: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                orderHeaders = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == userId,includeProperies:"ApplicationUser");
            }


                switch (status)
                {
                    case "pending":
                        orderHeaders = orderHeaders.Where(u => u.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
                        break;
                    case "inprocess":
                        orderHeaders = orderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusInProcess);
                        break;
                    case "completed":
                        orderHeaders = orderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusCompleted);
                        break;
                    case "approved":
                        orderHeaders = orderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusApproved);
                        break;
                    default:
                        break;


                }


            return Json(new { data = orderHeaders });
        }

        #endregion
    }
}
