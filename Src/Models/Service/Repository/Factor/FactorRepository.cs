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

                if (query.Contains("where"))
                {
                    query += $"and Status != {(byte)Factor.FactStatus.InBascket} ";
                }
                else
                {
                    query = $"where Status != {(byte) Factor.FactStatus.InBascket} ";
                }
                #endregion
                #region order
                query += $"order by {tableVar.OrderBy} {tableVar.OrderType}";
                #endregion
            }
            else
            {
                tableVar = new Common.TableVar();
                query = $"where Status != {(byte)Factor.FactStatus.InBascket}";
            }
            var data = Context.Tbl_Factor.SqlQuery($"Select * from Tbl_Factor {query}").ToList();

            #region make pegging
            data = data.Skip((tableVar.PageIndex - 1) * tableVar.PageSize)
                       .Take(tableVar.PageSize)
                       .ToList();
            #endregion

            var result = data.Adapt<List<Factor.ViewFullOrder>>();
            return result;
        }

        public List<Factor.ViewOrder> TopOrders()
        {
            var data = Context.Tbl_Factor.SqlQuery($"select top(5) * from Tbl_Factor where Status != {(byte)Factor.FactStatus.InBascket}").ToList();
            return data.Adapt<List<Factor.ViewOrder>>();
        }
    }
}