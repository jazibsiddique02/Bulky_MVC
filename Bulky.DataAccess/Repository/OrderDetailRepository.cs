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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetail
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(OrderDetail obj)
        {
            var objFromDb = _db.OrderDetails.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null) 
            {
                objFromDb.Count = obj.Count;
                objFromDb.Price = obj.Price;
                objFromDb.ProductId = obj.ProductId;
                objFromDb.OrderHeaderId = obj.OrderHeaderId;

            }
        }
    }
}
