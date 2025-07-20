using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderHeader : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);


        // method for updating just the status of order and payment
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);


        // method for updating just the session id and paymentintentid provided by stripe
        void UpdateStripePaymentId(int id, string paymentIntentId, string sessionId);
    }
}
