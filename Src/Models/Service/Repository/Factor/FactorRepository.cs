using Mapster;
using Src.Models.Data;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Src.Models.Service.Repository
{
    public class FactorRepository : GenericRepository<Tbl_Factor>, IFactorRepository
    {
        public FactorRepository(ALDBEntities context) : base(context) => OrderBy = item => item.SubmitDate;

        public List<Factor.ViewFullOrder> OrderList(Common.TableVar tableVar)
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
                        query += $"ID like '%{include}%' ";
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

            var Data = Context.Tbl_Factor.SqlQuery($"Select * from Tbl_Factor {query}").ToList();

            #region make pegging
            Data = Data.Skip((tableVar.PageIndex - 1) * tableVar.PageSize)
                       .Take(tableVar.PageSize)
                       .ToList();
            #endregion

            var resualt = Data.Adapt<List<Factor.ViewFullOrder>>();
            return resualt;
        }
    }
}