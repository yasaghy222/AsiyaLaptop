using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Linq;
using System.Web.Http;
using Mapster;
using Src.Models.Utitlity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;

namespace Src.Controllers.Api
{
    public class ProductController : BaseApiController
    {
        public ProductController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variable
        int AssignCount = 0;
        bool HasChild = false;
        Tbl_Product Proc = null;
        Tbl_ProcCat ProcCat = null;
        Tbl_ProcImg ProcImg = null;
        Tbl_ProcProp ProcProp = null;
        Tbl_PCPGroup PCPGroup = null;
        Tbl_ProcBrand ProcBrand = null;
        IEnumerable<Tbl_Product> ProcList = null;
        IEnumerable<Tbl_ProcCat> ProcCatList = null;
        IEnumerable<Tbl_ProcProp> ProcPropList = null;
        IEnumerable<Tbl_PCPGroup> PCPGroupList = null;
        #endregion

        #region product
        [HttpGet]
        public async Task<Common.Resualt> Get([FromUri]Common.TableVar tableVar)
        {
            Data = await Task.Run(() => _unitOfWork.Product.ProcList(tableVar));
            if (Data != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = new
                {
                    List = Data,
                    Count = await _unitOfWork.Product.GetCountAsync()
                };
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }
            return Resualt;
        }

        [HttpGet]
        public async Task<Common.Resualt> Detail(int id)
        {
            if (id != -1)
            {
                #region edit
                var Item = _unitOfWork.Product.SingleById(id).Adapt<Product.ViewTbl_Proc>();
                Item.CatList = GetCatList();
                Item.BrandList = GetBrandList();
                Data = Item;
                #endregion
            }
            else
            {
                #region add
                Data = new Product.ViewTbl_Proc
                {
                    ID = -1,
                    CatList = await Task.Run(() => GetCatList()),
                    BrandList = await Task.Run(() => _unitOfWork.ProcBrand.Get().Adapt<ICollection<Common.Select>>()),
                };
                #endregion
            }

            if (Data != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = Data;
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }
            return Resualt;
        }

        [HttpPost]
        public Common.Resualt AddEdit()
        {
            if (ModelState.IsValid)
            {
                FormData = HttpContext.Current.Request.Form;
                Proc = new Tbl_Product
                {
                    ID = int.Parse(FormData.Get("ID")),
                    Title = FormData.Get("Title"),
                    ShortDesc = FormData.Get("ShortDesc"),
                    FullDesc = FormData.Get("FullDesc"),
                    TopProp = FormData.Get("TopProp"),
                    Rate = byte.Parse(FormData.Get("Rate")),
                    Price = long.Parse(FormData.Get("Price")),
                    OffPrice = long.Parse(FormData.Get("OffPrice")),
                    OffID = int.Parse(FormData.Get("OffID")),
                    BrandID = int.Parse(FormData.Get("BrandID")),
                    CatID = int.Parse(FormData.Get("CatID")),
                    Count = int.Parse(FormData.Get("Count")),
                    Type = byte.Parse(FormData.Get("Type"))
                };

                if (Proc.ID == -1)
                {
                    #region add
                    Proc.ID = Proc.CatID + Function.GenerateNumCode();
                    _unitOfWork.Product.Add(Proc);
                    try
                    {
                        _unitOfWork.Save();
                        Resualt.Data = Proc.ID;
                        Resualt.Message = Function.UploadImg($"Product/{Proc.ID}");
                    }
                    catch (Exception)
                    {
                        Resualt.Message = Common.ResualtMessage.InternallServerError;
                    }
                    #endregion
                }
                else
                {
                    #region edit
                    _unitOfWork.Product.Update(Proc);
                    try
                    {
                        _unitOfWork.Save();
                        Resualt.Data = Proc.ID;
                        Resualt.Message = Function.UpdateImg(Proc.ID, "Product");
                    }
                    catch (Exception)
                    {
                        Resualt.Message = Common.ResualtMessage.InternallServerError;
                    }
                    #endregion
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }
            return Resualt;
        }
        #endregion

        #region category
        [HttpGet]
        public Common.Resualt CatList()
        {
            ProcCatList = _unitOfWork.ProcCat.Get();
            if (ProcCatList != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = ProcCatList.Adapt<List<Common.FullTree>>();
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }
            return Resualt;
        }

        [HttpGet]
        public async Task<Common.Resualt> CatDetail(int id)
        {
            if (id != -1)
            {
                #region edit
                var Item = _unitOfWork.ProcCat.SingleById(id).Adapt<Product.ViewTbl_ProcCat>();
                Item.CatList = GetCatList();
                Data = Item;
                #endregion
            }
            else
            {
                #region add
                Data = new Product.ViewTbl_ProcCat
                {
                    ID = -1,
                    CatList = await Task.Run(() => GetCatList()),
                };
                #endregion
            }

            if (Data != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = Data;
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }
            return Resualt;
        }

        [HttpPost]
        public async Task<Common.Resualt> AddEditCat([FromBody] Product.ViewTbl_ProcCat model)
        {
            if (ModelState.IsValid)
            {
                ProcCat = await _unitOfWork.ProcCat.SingleByIdAsync(model.ID);
                if (ProcCat == null)
                {
                    #region add
                    ProcCat = model.Adapt<Tbl_ProcCat>();
                    ProcCat.PID = ProcCat.PID == 0 ? null : ProcCat.PID;
                    _unitOfWork.ProcCat.Add(ProcCat);
                    #endregion
                }
                else
                {
                    #region edit
                    HasChild = await Task.Run(() => GenericFunction<Tbl_ProcCat>
                                                    .HasChild(ProcCat, item => item.PID == ProcCat.ID));
                    AssignCount = ProcCat.Tbl_Product.Count;

                    if (!HasChild && AssignCount == 0)
                    {
                        ProcCat.PID = model.PID;
                    }
                    ProcCat.Title = model.Title;
                    #endregion
                }

                try
                {
                    await _unitOfWork.SaveAsync();
                    #region change product catId
                    if (ProcCat.PID != null)
                    {
                        ProcList = await _unitOfWork.Product.GetAsync(item => item.CatID == ProcCat.PID);
                        foreach (Tbl_Product item in ProcList)
                        {
                            item.CatID = ProcCat.ID;
                        }
                        await _unitOfWork.SaveAsync();
                    }
                    #endregion
                    Resualt.Message = Common.ResualtMessage.OK;
                    Resualt.Data = ProcCat.ID;
                }
                catch (Exception)
                {
                    Resualt.Message = Common.ResualtMessage.InternallServerError;
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }
            return Resualt;
        }

        [HttpPost]
        public async Task<Common.Resualt> DelCat([FromBody]int id)
        {
            if (ModelState.IsValid)
            {
                ProcCat = await _unitOfWork.ProcCat.SingleByIdAsync(id);
                if (ProcCat != null)
                {
                    if (ProcCat.Tbl_Product.Count == 0)
                    {
                        _unitOfWork.ProcCat.Remove(ProcCat);
                        try
                        {
                            await _unitOfWork.SaveAsync();
                            Resualt.Message = Common.ResualtMessage.OK;
                        }
                        catch (Exception)
                        {
                            Resualt.Message = "شما تنها مجاز به حذف گروه هایی هستید که محصولی با این گروه وجود نداشته باشد.";
                        }
                    }
                }
                else
                {
                    Resualt.Message = Common.ResualtMessage.NotFound;
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }

            return Resualt;
        }
        #endregion

        #region brand
        [HttpGet]
        public Common.Resualt BrandList()
        {
            Data = _unitOfWork.ProcBrand.Get();
            if (Data != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = Data.Adapt<List<Product.ViewTbl_ProcBrand>>();
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }
            return Resualt;
        }

        [HttpPost]
        public Common.Resualt AddEditBrand()
        {
            if (ModelState.IsValid)
            {
                FormData = HttpContext.Current.Request.Form;
                ProcBrand = new Tbl_ProcBrand
                {
                    ID = int.Parse(FormData.Get("ID")),
                    Title = FormData.Get("Title")
                };

                if (ProcBrand.ID == -1)
                {
                    #region add
                    _unitOfWork.ProcBrand.Add(ProcBrand);
                    try
                    {
                        _unitOfWork.Save();
                        Resualt.Message = Function.UploadImg($"ProcBrand/{ProcBrand.ID}");
                    }
                    catch (Exception)
                    {
                        Resualt.Message = Common.ResualtMessage.InternallServerError;
                    }
                    #endregion
                }
                else
                {
                    #region edit
                    _unitOfWork.ProcBrand.Update(ProcBrand);
                    try
                    {
                        _unitOfWork.Save();
                        Resualt.Message = Function.UpdateImg(ProcBrand.ID,"ProcBrand");
                    }
                    catch (Exception)
                    {
                        Resualt.Message = Common.ResualtMessage.InternallServerError;
                    }
                    #endregion
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }

            return Resualt;
        }

        [HttpPost]
        public async Task<Common.Resualt> DelBrand([FromBody]int id)
        {
            if (ModelState.IsValid)
            {
                ProcBrand = await _unitOfWork.ProcBrand.SingleByIdAsync(id);
                if (ProcBrand != null)
                {
                    AssignCount = ProcBrand.Tbl_Product.Count;
                    if (AssignCount == 0)
                    {
                        _unitOfWork.ProcBrand.Remove(ProcBrand);
                        try
                        {
                            await _unitOfWork.SaveAsync();
                            Function.DelImg(ProcBrand.ID, "ProcBrand");
                            Resualt.Message = Common.ResualtMessage.OK;
                        }
                        catch (Exception)
                        {
                            Resualt.Message = Common.ResualtMessage.InternallServerError;
                        }
                    }
                    else
                    {
                        Resualt.Message = Common.ResualtMessage.ChildAssignError;
                    }
                }
                else
                {
                    Resualt.Message = Common.ResualtMessage.NotFound;
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }

            return Resualt;
        }
        #endregion

        #region image
        [HttpGet]
        public Common.Resualt ImgList([FromUri] int procID)
        {
            Data = _unitOfWork.ProcImg.Get(item => item.ProcID == procID);
            if (Data != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = Data.Adapt<List<Common.SelectWithProc>>();
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }

            return Resualt;
        }

        [HttpPost]
        public Common.Resualt AddEditImg()
        {
            if (ModelState.IsValid)
            {
                FormData = HttpContext.Current.Request.Form;
                ProcImg = new Tbl_ProcImg
                {
                    ID = int.Parse(FormData.Get("ID")),
                    ProcID = int.Parse(FormData.Get("ProcID")),
                    Title = FormData.Get("Title")
                };

                if (ProcImg.ID == -1)
                {
                    #region add
                    _unitOfWork.ProcImg.Add(ProcImg);
                    try
                    {
                        _unitOfWork.Save();
                        Resualt.Message = Function.UploadImg($"Product/{ProcImg.ProcID}_{ProcImg.ID}");
                    }
                    catch (Exception)
                    {
                        Resualt.Message = Common.ResualtMessage.InternallServerError;
                    }
                    #endregion
                }
                else
                {
                    #region edit
                    _unitOfWork.ProcImg.Update(ProcImg);
                    try
                    {
                        _unitOfWork.Save();
                        Resualt.Message = Function.UpdateImg($"{ProcImg.ProcID}_{ ProcImg.ID}", "Product");
                    }
                    catch (Exception)
                    {
                        Resualt.Message = Common.ResualtMessage.InternallServerError;
                    }
                    #endregion
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }

            return Resualt;
        }

        [HttpPost]
        public Common.Resualt DelImg([FromBody]int id)
        {
            if (ModelState.IsValid)
            {
                ProcImg = _unitOfWork.ProcImg.SingleById(id);
                _unitOfWork.ProcImg.Remove(ProcImg);
                try
                {
                    _unitOfWork.Save();
                    Function.DelImg(ProcImg.ProcID + ProcImg.ID, "Product");
                    Resualt.Message = Common.ResualtMessage.OK;
                }
                catch (Exception)
                {
                    Resualt.Message = Common.ResualtMessage.InternallServerError;
                    throw;
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }

            return Resualt;
        }
        #endregion

        #region property
        [HttpGet]
        public async Task<Common.Resualt> PropList([FromUri] int procID, [FromUri] int catID)
        {
            PCPGroupList = await _unitOfWork.PCPGroup.GetAsync(item => item.CatID == catID);
            ProcPropList = await _unitOfWork.ProcProp.GetAsync(item => item.ProcID == procID);

            List<Product.ViewProcProp> Props = new List<Product.ViewProcProp>();
            foreach (Tbl_PCPGroup item in PCPGroupList)
            {
                Product.ViewProcProp Prop = new Product.ViewProcProp
                {
                    ID = item.ID,
                    PID = item.PID,
                    Title = item.Title,
                    HasChild = PCPGroupList.Count(x => x.PID == item.ID) > 0 ? true : false
                };

                if (ProcPropList.Count() > 0)
                {
                    bool isExist = ProcPropList.Any(x => x.PCPGID == item.ID);
                    if (isExist)
                    {
                        ProcProp = ProcPropList.Single(x => x.PCPGID == item.ID);
                        Prop.Value = ProcProp.Value;
                    }
                }
                Props.Add(Prop);
            }
            if (Props != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = Props;
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }

            return Resualt;
        }

        [HttpPost]
        public async Task<Common.Resualt> EditProp([FromBody]Product.ViewPropVar model)
        {
            if (ModelState.IsValid)
            {
                ProcProp = await _unitOfWork.ProcProp
                                            .SingleAsync(item =>
                                                         item.PCPGID == model.PCPGID &&
                                                         item.ProcID == model.ProcID);
                if (ProcProp != null)
                {
                    #region edit
                    ProcProp.Value = model.Value;
                    #endregion
                }
                else
                {
                    #region add
                    ProcProp = model.Adapt<Tbl_ProcProp>();
                    _unitOfWork.ProcProp.Add(ProcProp);
                    #endregion
                }

                try
                {
                    _unitOfWork.Save();
                    Resualt.Message = Common.ResualtMessage.OK;
                }
                catch (Exception)
                {
                    Resualt.Message = Common.ResualtMessage.InternallServerError;
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }

            return Resualt;
        }
        #endregion

        #region product category property group
        [HttpGet]
        public Common.Resualt PCPGList([FromUri] int catID)
        {
            Data = _unitOfWork.PCPGroup.Get(item => item.CatID == catID);
            if (Data != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = Data.Adapt<List<Common.FullTree>>();
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }

            return Resualt;
        }

        [HttpPost]
        public async Task<Common.Resualt> AddEditPCPG([FromBody]Product.ViewTbl_PCPGroup model)
        {
            if (ModelState.IsValid)
            {
                PCPGroup = model.Adapt<Tbl_PCPGroup>();

                if (model.ID == -1)
                {
                    #region add
                    PCPGroup.PID = PCPGroup.PID == 0 ? null : PCPGroup.PID;
                    _unitOfWork.PCPGroup.Add(PCPGroup);
                    #endregion
                }
                else
                {
                    #region edit
                    _unitOfWork.PCPGroup.Update(PCPGroup);
                    #endregion
                }

                try
                {
                    await _unitOfWork.SaveAsync();
                    Resualt.Message = Common.ResualtMessage.OK;
                }
                catch (Exception)
                {
                    Resualt.Message = Common.ResualtMessage.InternallServerError;
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }

            return Resualt;
        }

        [HttpPost]
        public async Task<Common.Resualt> DelPCPG([FromBody]int id)
        {
            if (ModelState.IsValid)
            {
                PCPGroup = await _unitOfWork.PCPGroup.SingleByIdAsync(id);
                HasChild = await Task.Run(() => GenericFunction<Tbl_PCPGroup>
                                               .HasChild(PCPGroup, item => item.PID == PCPGroup.ID));
                AssignCount = PCPGroup.Tbl_ProcProp.Count;

                if (!HasChild && AssignCount == 0)
                {

                    _unitOfWork.PCPGroup.Remove(PCPGroup);
                    try
                    {
                        _unitOfWork.Save();
                        Resualt.Message = Common.ResualtMessage.OK;
                    }
                    catch (Exception)
                    {
                        Resualt.Message = Common.ResualtMessage.InternallServerError;
                    }
                }
                else
                {
                    Resualt.Message = Common.ResualtMessage.ChildAssignError;
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }

            return Resualt;
        }
        #endregion
    }
}
