using Src.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Src.Models.ViewData.Base;
using System.Reflection;

namespace Src.Models.Service.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        #region variable
        protected ALDBEntities Context { get; set; }
        protected DbSet<T> Dbset { get; set; }
        protected Expression<Func<T, long>> OrderByL { get; set; } = null;
        protected Expression<Func<T, byte>> OrderByB { get; set; } = null;
        protected Expression<Func<T, object>> OrderBy { get; set; } = null;
        protected Func<IQueryable<T>, IOrderedQueryable<T>> Order { get; set; } = null;
        protected IQueryable<T> Query { get; set; } = null;
        #endregion

        public GenericRepository(ALDBEntities context)
        {
            Context = context;
            Query = Dbset = context.Set<T>();
        }

        #region select
        public T Single(Expression<Func<T, bool>> where) => Dbset.FirstOrDefault(where);
        public async Task<T> SingleAsync(Expression<Func<T, bool>> where) => await Dbset.FirstOrDefaultAsync(where);
        public bool SingleAny(Expression<Func<T, bool>> where) => Dbset.Any(where);
        public async Task<bool> SingleAnyAsync(Expression<Func<T, bool>> where) => await Dbset.AnyAsync(where);
        public long GetCount() => Dbset.Count();
        public long GetCount(Expression<Func<T, bool>> where) => Dbset.Count(where);
        public async Task<long> GetCountAsync() => await Dbset.CountAsync();
        public async Task<long> GetCountAsync(Expression<Func<T, bool>> where) => await Dbset.CountAsync(where);
        public T SingleById(object id) => Dbset.Find(id);
        public async Task<T> SingleByIdAsync(object id) => await Dbset.FindAsync(id);
        public IEnumerable<T> Get(Expression<Func<T, bool>> where = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includes = "")
        {
            IQueryable<T> query = Dbset;

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
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includes = "")
        {
            IQueryable<T> query = Dbset;

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
        public IEnumerable<T> Get(Common.TableVar tableVar)
        {
            string query = "";

            if (tableVar != null)
            {
                #region filler
                if (tableVar.Includes != null)
                {
                    query = "where Title ";
                    foreach (string include in tableVar.Includes.Split(','))
                    {
                        query += $"like '%{include}%'";
                    }
                }
                #endregion
                #region order
                query += $"order by {tableVar.OrderBy} {tableVar.OrderType}";
                #endregion
            }
            else
            {
                tableVar = new Common.TableVar();
            }

            var Data = Context.Database.SqlQuery<T>($"Select * from {typeof(T).Name} {query}").ToList();

            #region make pegging
            Data = Data.Skip((tableVar.PageIndex - 1) * tableVar.PageSize)
                       .Take(tableVar.PageSize)
                       .ToList();
            #endregion

            return Data;
        }
        public async Task<IEnumerable<T>> GetAsync(Common.TableVar tableVar)
        {
            #region get prop
            PropertyInfo prop = null;
            if (tableVar.OrderBy == "first")
            {
                prop = Query.ElementType.GetProperties().First();
            }
            else
            {
                prop = Query.ElementType.GetProperties().Single(item => item.Name == tableVar.OrderBy);
            }
            #endregion

            #region make paging
            IEnumerable<T> Data = await Query.ToListAsync();
            Data = Data.Skip((tableVar.PageIndex - 1) * tableVar.PageSize)
                       .Take(tableVar.PageSize);
            #endregion

            #region order

            Data = tableVar.OrderType == "" ? Data.OrderBy(item => prop) : Data.OrderByDescending(item => prop);
            #endregion

            return Data;
        }
        #endregion
        #region add
        public void Add(T entity) => Dbset.Add(entity);
        public void AddRange(IEnumerable<T> entity) => Dbset.AddRange(entity);
        #endregion
        #region update
        public void Update(T entity)
        {
            Dbset.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }
        public void UpdateRange(IEnumerable<T> entitys)
        {
            foreach (T item in entitys)
            {
                Dbset.Attach(item);
            }
            Context.Entry(entitys).State = EntityState.Modified;
        }
        #endregion
        #region delete
        public void Remove(T entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                Dbset.Attach(entity);
            }
            Dbset.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entitys)
        {
            if (Context.Entry(entitys).State == EntityState.Detached)
            {
                foreach (T item in entitys)
                {
                    Dbset.Attach(item);
                }
            }
            Dbset.RemoveRange(entitys);
        }
        #endregion
    }
}