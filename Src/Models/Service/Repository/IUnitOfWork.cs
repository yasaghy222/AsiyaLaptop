using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Src.Models.Service.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        #region product
        IProductRepository Product { get;}
        #endregion

        int Save();
        Task<int> SaveAsync();
    }
}