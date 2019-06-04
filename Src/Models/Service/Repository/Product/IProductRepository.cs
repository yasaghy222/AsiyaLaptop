using Src.Models.Data;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Src.Models.Service.Repository
{
    public interface IProductRepository : IGenericRepository<Tbl_Product>
    {
        List<Product.ViewProc> ProcList(Common.TableVar tableVar);

        #region search
        Task<List<Product.SearchResult>> Search(string title);
        Task<Tuple<List<Product.FullSearchResult>, string>> Search(Product.SearchParam searchParam);
        Task<long> GetMaxPrice(string catName);
        #endregion
    }
}