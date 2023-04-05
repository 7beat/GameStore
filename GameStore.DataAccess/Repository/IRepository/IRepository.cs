using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T GetFirstOrDefault(Expression<Func<T, bool>> predicate, params string[] includeProperties);
        IEnumerable<T> GetAll(params string[] includeProperties);
        void Add(T entity);
        void Remove(T entity);

        #region Async

        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params string[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(params string[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params string[] includeProperties);

        #endregion
    }
}
