using Src.Models.ViewData.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Src.Models.Service.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        #region select
        T Single(Expression<Func<T, bool>> where);
        Task<T> SingleAsync(Expression<Func<T, bool>> where);
        bool SingleAny(Expression<Func<T, bool>> where);
        Task<bool> SingleAnyAsync(Expression<Func<T, bool>> where);
        T SingleById(object id);
        Task<T> SingleByIdAsync(object id);
        long GetCount();
        long GetCount(Expression<Func<T, bool>> where);
        Task<long> GetCountAsync();
        Task<long> GetCountAsync(Expression<Func<T, bool>> where);
        IEnumerable<T> Get(Expression<Func<T, bool>> where = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includes = "");
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includes = "");
        IEnumerable<T> Get(Common.TableVar tableVar);
        Task<IEnumerable<T>> GetAsync(Common.TableVar tableVar);
        #endregion
        #region add
        void Add(T entity);
        void AddRange(IEnumerable<T> entity);
        #endregion
        #region update
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entitys);
        #endregion
        #region delete
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entitys);
        #endregion
    }

}