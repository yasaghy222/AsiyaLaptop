using Mapster;
using Src.Models.Data;
using Src.Models.ViewData.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Models.Service.Repository
{
    public class PCPGRepository : GenericRepository<Tbl_PCPGroup>, IPCPGRepository
    {
        public PCPGRepository(ALDBEntities context) : base(context) { }

        /// <summary>
        /// get list of group with parent list
        /// </summary>
        /// <param name="id"> category id</param>
        /// <returns></returns>
        public async Task<IEnumerable<Tbl_PCPGroup>> GetGroups(int? id)
        {
            if (id != null)
            {
                var list = await Task.Run(() => Context.Tbl_PCPGroup.Where(item => item.CatID == id).ToList());
                bool isRoot = await Task.Run(() => Context.Tbl_ProcCat.Any(item => item.PID == id));
                if (!isRoot)
                {
                    int? pID = await Task.Run(() => Context.Tbl_ProcCat.SingleOrDefault(item => item.ID == id)?.PID);
                    var pList = await GetGroups(pID);
                    list.AddRange(pList);
                }
                return list;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// get list of product pcpgroup with property
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<List<Product.ViewPCPGroup>> GetPCPGs(Tbl_Product product)
        {
            List<Product.ViewPCPGroup> PCPGroups = new List<Product.ViewPCPGroup>();
            var PCPGroupList = await GetGroups(product.CatID);
            foreach (Tbl_PCPGroup i in PCPGroupList.Where(x => x.PID == null))
            {
                if (i.Tbl_PCPGroup1.Count() > 0)
                {
                    Product.ViewPCPGroup PCPGroup = new Product.ViewPCPGroup
                    {
                        Title = i.Title,
                        EnTitle = i.EnTitle
                    };

                    PCPGroup.Props = new List<Product.ViewProcProp>();
                    foreach (var j in i.Tbl_PCPGroup1)
                    {
                        Product.ViewProcProp procProp = new Product.ViewProcProp
                        {
                            Title = j.Title,
                            EnTitle = j.EnTitle,
                            Value = j.Tbl_ProcProp.SingleOrDefault(item => item.ProcID == product.ID).Value
                        };
                        PCPGroup.Props.Add(procProp);
                    }

                    PCPGroups.Add(PCPGroup);
                }
            }
            return PCPGroups;
        }
    }
}