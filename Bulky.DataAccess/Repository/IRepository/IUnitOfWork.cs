using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }

        IProductRepository Product { get; }

        ICompanyRepository Company { get; }

        IShoppingCart ShoppingCart { get; }

        IOrderHeader OrderHeader { get; }

        IOrderDetail OrderDetail { get; }

        IProductImage ProductImage { get; }



        IApplicationUserRepository ApplicationUser { get; }
        public void Save();
   
    }
}
