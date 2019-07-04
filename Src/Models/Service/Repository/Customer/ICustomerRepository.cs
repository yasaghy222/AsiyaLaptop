using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Src.Models.Service.Repository
{
    public interface ICustomerRepository : IGenericRepository<Data.Tbl_Customer>
    {
        List<Customer.ViewTbl_Customer> CustList(Common.TableVar tableVar);
    }
}