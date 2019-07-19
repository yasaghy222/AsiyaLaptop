using Src.Models.Data;
using Src.Models.ViewData.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Src.Models.Service.Repository
{
    public interface IPCPGRepository : IGenericRepository<Tbl_PCPGroup>
    {
        Task<IEnumerable<Tbl_PCPGroup>> GetGroups(int? catID);
        Task<List<Product.ViewPCPGroup>> GetPCPGs(Tbl_Product product);
    }
}