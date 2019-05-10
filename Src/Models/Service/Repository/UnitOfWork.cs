using Src.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Src.Models.Service.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ALDBEntities _context;
        public UnitOfWork(ALDBEntities context)
        {
            _context = context;
        }

        #region product
        private IProductRepository product;
        public IProductRepository Product { get => product = product ?? new ProductRepository(_context); }
        #endregion

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
    }
}