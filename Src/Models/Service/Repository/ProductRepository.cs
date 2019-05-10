using Src.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Src.Models.Service.Repository
{
    public class ProductRepository : GenericRepository<Tbl_Product>, IProductRepository
    {
        public ProductRepository(ALDBEntities context) : base(context) { }
    }
}