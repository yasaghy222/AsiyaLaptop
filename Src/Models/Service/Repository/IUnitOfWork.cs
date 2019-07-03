using Src.Models.Data;
using System;
using System.Threading.Tasks;

namespace Src.Models.Service.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        #region product
        IProductRepository Product { get;}
        IGenericRepository<Tbl_ProcCat> ProcCat { get;}
        IGenericRepository<Tbl_ProcBrand> ProcBrand { get; }
        IGenericRepository<Tbl_ProcImg> ProcImg { get; }
        IGenericRepository<Tbl_ProcProp> ProcProp { get; }
        IGenericRepository<Tbl_PCPGroup> PCPGroup { get; }
        #endregion

        #region factor
        IFactorRepository Factor { get; }
        #endregion

        #region customer
        ICustomerRepository Customer { get; }
        IGenericRepository<Tbl_CustAddress> CustAddress { get; }
        #endregion

        #region menu
        IGenericRepository<Tbl_Menu> Menu { get; }
        #endregion

        #region media
        IGenericRepository<Tbl_Media> Media { get;}
        #endregion

        #region newsletter
        IGenericRepository<Tbl_Newsletter> Newsletter { get; }
        #endregion

        int Save();
        Task<int> SaveAsync();
    }
}