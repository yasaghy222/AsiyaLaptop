using Src.Models.Data;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System.Collections.Generic;

namespace Src.Models.Service.Repository
{
    public interface IProductRepository : IGenericRepository<Tbl_Product>
    {
        List<Product.ViewProc> ProcList(Common.TableVar tableVar);
    }
}