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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCart
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Update(ShoppingCart cart)
        {
            var objFromDb = _db.ShoppingCart.FirstOrDefault(x=>x.Id == cart.Id);
            if (objFromDb != null) 
            {
                objFromDb.Id = cart.Id;
                objFromDb.ProductId = cart.ProductId;
                objFromDb.ApplicationUserId = cart.ApplicationUserId;
                objFromDb.Count = cart.Count;
            
            }
        }
    }
}
