using Src.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Src.Models.Service.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private ALDBEntities _context;
        private DbSet<T> _dbset;

        public GenericRepository(ALDBEntities context)
        {
            _context = context;
            _dbset = context.Set<T>();
        }
        #region select
        public virtual T Single(Expression<Func<T, bool>> where) => _dbset.FirstOrDefault(where);
        public virtual async Task<T> SingleAsync(Expression<Func<T, bool>> where) => await _dbset.FirstOrDefaultAsync(where);
        public virtual bool SingleAny(Expression<Func<T, bool>> where) => _dbset.Any(where);
        public virtual async Task<bool> SingleAnyAsync(Expression<Func<T, bool>> where) => await _dbset.AnyAsync(where);
        public virtual long GetCount() => _dbset.Count();
        public virtual long GetCount(Expression<Func<T, bool>> where) => _dbset.Count(where);
        public virtual async Task<long> GetCountAsync() => await _dbset.CountAsync();
        public virtual async Task<long> GetCountAsync(Expression<Func<T, bool>> where) => await _dbset.CountAsync(where);
        public virtual T SingleById(object id) => _dbset.Find(id);
        public virtual async Task<T> SingleByIdAsync(object id) => await _dbset.FindAsync(id);
        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> where = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includes = "")
        {
            IQueryable<T> query = _dbset;

            if (where != null)
            {
                query = query.Where(where);
            }

            if (orderby != null)
            {
                query = orderby(query);
            }

            if (includes != "")
            {
                foreach (string include in includes.Split(','))
                {
                    query = query.Include(include);
                }
            }


            return query.ToList();
        }

        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includes = "")
        {
            IQueryable<T> query = _dbset;

            if (where != null)
            {
                query = query.Where(where);
            }

            if (orderby != null)
            {
                query = orderby(query);
            }

            if (includes != "")
            {
                foreach (string include in includes.Split(','))
                {
                    query = query.Include(include);
                }
            }

            return await query.ToListAsync();
        }
        #endregion
        #region add
        public virtual void Add(T entity) => _dbset.Add(entity);
        public virtual void AddRange(IEnumerable<T> entity) => _dbset.AddRange(entity);
        #endregion
        #region delete
        public virtual void Remove(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbset.Attach(entity);
            }
            _dbset.Remove(entity);
        }
        public virtual void RemoveRange(IEnumerable<T> entitys)
        {
            if (_context.Entry(entitys).State == EntityState.Detached)
            {
                foreach (T item in entitys)
                {
                    _dbset.Attach(item);
                }
            }
            _dbset.RemoveRange(entitys);
        }
        #endregion
        #region update
        public virtual void Update(T entity)
        {
            _dbset.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public virtual void UpdateRange(IEnumerable<T> entitys)
        {
            foreach (T item in entitys)
            {
                _dbset.Attach(item);
            }
            _context.Entry(entitys).State = EntityState.Modified;
        }
        #endregion
    }
}