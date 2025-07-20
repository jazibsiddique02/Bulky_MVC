using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeader
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            var objFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.StreetAddress = obj.StreetAddress;
                objFromDb.City = obj.City;
                objFromDb.State = obj.State;
                objFromDb.PostalCode = obj.PostalCode;
                objFromDb.PhoneNumber = obj.PhoneNumber;
                objFromDb.ApplicationUserId = obj.ApplicationUserId;
                objFromDb.Carrier = obj.Carrier;
                objFromDb.PaymentDueDate = obj.PaymentDueDate;
                objFromDb.PaymentIntentId = obj.PaymentIntentId;
                objFromDb.PaymentDate = obj.PaymentDate;
                objFromDb.OrderTotal = obj.OrderTotal;
                objFromDb.PaymentStatus = obj.PaymentStatus;
                objFromDb.TrackingNumber = obj.TrackingNumber;
                objFromDb.ShippingDate = obj.ShippingDate;
                objFromDb.OrderDate = obj.OrderDate;
                objFromDb.OrderStatus = obj.OrderStatus;
            }
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            OrderHeader objFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (objFromDb != null)
            {
                objFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    objFromDb.PaymentStatus = paymentStatus;
                }
            }

        }

        public void UpdateStripePaymentId(int id, string paymentIntentId, string sessionId)
        {
            OrderHeader objFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (objFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    objFromDb.SessionId = sessionId;
                }

                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    objFromDb.PaymentIntentId = paymentIntentId;
                    objFromDb.PaymentDate = DateTime.Now;
                }
            }
        }

    }
}
