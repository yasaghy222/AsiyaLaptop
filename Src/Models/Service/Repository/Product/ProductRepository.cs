using Src.Models.Data;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Mapster;
using System.Linq.Expressions;
using Src.Models.Utitlity;
using Extensions = Src.Models.Utitlity.Extensions;

namespace Src.Models.Service.Repository
{
    public class ProductRepository : GenericRepository<Tbl_Product>, IProductRepository
    {
        public ProductRepository(ALDBEntities context) : base(context) => OrderBy = item => item.Title;

        public List<Product.ViewProc> ProcList(Common.TableVar tableVar)
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
                        query += $"Title like '%{include}%' ";
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

            var Data = Context.Tbl_Product.SqlQuery($"Select * from Tbl_Product {query}").ToList();

            #region make pegging
            Data = Data.Skip((tableVar.PageIndex - 1) * tableVar.PageSize)
                       .Take(tableVar.PageSize)
                       .ToList();
            #endregion

            var resualt = Data.Adapt<List<Product.ViewProc>>();
            return resualt;
        }
    }
}