using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T - Category or any other generic model
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T,bool>> filter);   // Get using first or default method. example: _db.Categories.FirstOrDefault(x => x.id == id)

        void Add(T entity);

        // Update method is not used here because logic of update for different models like category, product,etc. might be different from one another.

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);

    }
}
