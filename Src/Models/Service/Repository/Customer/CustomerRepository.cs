using Mapster;
using Src.Models.Data;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Src.Models.Service.Repository
{
    public class CustomerRepository : GenericRepository<Tbl_Customer>, ICustomerRepository
    {
        public CustomerRepository(ALDBEntities contect) : base(contect) { }

        public List<Customer.ViewTbl_Customer> CustList(Common.TableVar tableVar)
        {
            string query = "";

            if (tableVar != null)
            {
                #region filler
                if (tableVar.Includes != null)
                {
                    int i = 0;
                    query = "where ";
                    foreach (string include in tableVar.Includes.Split(','))
                    {
                        if (i > 0)
                        {
                            query += "or ";
                        }
                        query += $"Phone like '%{include}%' ";
                        i++;
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

            var Data = Context.Tbl_Customer.SqlQuery($"Select * from Tbl_Customer {query}").ToList();

            try
            {
                #region make pegging
                Data = Data.Skip((tableVar.PageIndex - 1) * tableVar.PageSize)
                           .Take(tableVar.PageSize)
                           .ToList();
                #endregion

                var resualt = Data.Adapt<List<Customer.ViewTbl_Customer>>();
                return resualt;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}