using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Src.Models.Service.Repository
{
    public interface IFactorRepository : IGenericRepository<Data.Tbl_Factor>
    {
        List<Factor.ViewFullOrder> OrderList(Common.TableVar tableVar);
    }
}