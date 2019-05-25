using Src.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Src.Models.Service.Repository
{
    public class CustomerRepository : GenericRepository<Tbl_Customer>,ICustomerRepository
    {
        public CustomerRepository(ALDBEntities contect) : base(contect) { }
    }
}