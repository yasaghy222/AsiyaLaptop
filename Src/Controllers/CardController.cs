using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;

namespace Src.Controllers
{
    public class CardController : BaseController
    {
        public CardController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variables
        private int? custId;
        private Tbl_Factor tblFactor;
        #endregion

        #region Card
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            #region Get Customer ID
            var info = Function.GetCustInfo(Request);
            var hashToken = Function.GenerateHash(info.Token);
            var custId = await Task.Run(() => _unitOfWork.Customer.Single(item => item.Token == hashToken).ID);
            if (custId == 0) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            var data = await _unitOfWork.Factor.SingleAsync(item =>
                item.CustID == custId && item.Status == (byte)Factor.FactStatus.InBascket);
            return View(data.Adapt<Factor.ViewCard>());
        }

        [HttpPost]
        public async Task<JsonResult> Get()
        {
            #region Get Customer ID
            var info = Function.GetCustInfo(Request);
            var hashToken = Function.GenerateHash(info.Token);
            var custId = await Task.Run(() => _unitOfWork.Customer.Single(item => item.Token == hashToken).ID);
            if (custId == 0) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            var data = await _unitOfWork.Factor.SingleAsync(item =>
                item.CustID == custId && item.Status == (byte)Factor.FactStatus.InBascket);
            return data != null
                ? new JsonResult
                {
                    Data = new Common.Result
                    {
                        Data = data.Adapt<Factor.ViewCard>(),
                        Message = Common.ResultMessage.OK
                    }
                }
                : new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };

        }

        [HttpPost]
        public async Task<JsonResult> AddOrEdit(int procId = 0)
        {
            if (procId == 0) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };

            #region Get Product
            var proc = await _unitOfWork.Product.SingleByIdAsync(procId);
            if (proc == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            #region Get Customer ID
            var info = Function.GetCustInfo(Request);
            var hashToken = Function.GenerateHash(info.Token);
            custId = await Task.Run(() => _unitOfWork.Customer.Single(item => item.Token == hashToken)?.ID);
            if (custId == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            #region Get Customer Factor
            tblFactor = await _unitOfWork.Factor.SingleAsync(item =>
                item.CustID == custId && item.Status == (byte)Factor.FactStatus.InBascket);
            #endregion

            #region Add Or Edit Customer Factor Items
            if (tblFactor == null)
            {
                #region Add
                var factorID = custId + Function.GenerateNumCode();
                var factor = new Tbl_Factor
                {
                    ID = factorID ?? 0,
                    CustID = custId ?? 0,
                    Status = (byte)Factor.FactStatus.InBascket,
                    TotalPrice = (proc.Price - proc.OffPrice),
                    IsPrint = false,
                    SubmitDate = DateTime.Now
                };
                var factProc = new Tbl_FactProc
                {
                    Count = 1,
                    FactID = factorID ?? 0,
                    ProcID = proc.ID
                };
                using var aldb = new ALDBEntities();
                using var dbTransaction = aldb.Database.BeginTransaction();
                try
                {
                    aldb.Tbl_Factor.Add(factor);
                    aldb.Tbl_FactProc.Add(factProc);
                    await aldb.SaveChangesAsync();
                    dbTransaction.Commit();
                    return new JsonResult { Data = new Common.Result(Common.ResultMessage.OK) };
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return new JsonResult { Data = new Common.Result(Common.ResultMessage.InternallServerError) };
                }
                #endregion
            }
            else
            {
                #region Edit
                var factProc = await _unitOfWork.FactProc
                                                .SingleAsync(item => item.ProcID == proc.ID &&
                                                                          item.FactID == tblFactor.ID);
                if (factProc == null)
                {
                    #region add product to factor
                    factProc = new Tbl_FactProc
                    {
                        Count = 1,
                        FactID = tblFactor.ID,
                        ProcID = proc.ID
                    };
                    _unitOfWork.FactProc.Add(factProc);
                    #endregion
                }
                else
                {
                    #region pulse factor product count
                    factProc.Count++;
                    #endregion
                }

                tblFactor.TotalPrice += (proc.Price - proc.OffPrice) * factProc.Count;

                try
                {
                    await _unitOfWork.SaveAsync();
                    return new JsonResult { Data = new Common.Result(Common.ResultMessage.OK) };
                }
                catch (Exception)
                {
                    return new JsonResult { Data = new Common.Result(Common.ResultMessage.InternallServerError) };
                }
                #endregion
            }
            #endregion
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int procId = 0)
        {
            if (procId == 0) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };

            #region Get Product
            var proc = await _unitOfWork.Product.SingleByIdAsync(procId);
            if (proc == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            #region Get Customer ID
            var info = Function.GetCustInfo(Request);
            var hashToken = Function.GenerateHash(info.Token);
            custId = await Task.Run(() => _unitOfWork.Customer.Single(item => item.Token == hashToken)?.ID);
            if (custId == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            #region Get Customer Factor
            tblFactor = await _unitOfWork.Factor.SingleAsync(item =>
                item.CustID == custId && item.Status == (byte)Factor.FactStatus.InBascket);
            #endregion

            #region delete item from factor
            var factProc = tblFactor.Tbl_FactProc.SingleOrDefault(item => item.ProcID == proc.ID);
            if (factProc == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            _unitOfWork.FactProc.Remove(factProc);

            if (tblFactor.Tbl_FactProc.Count == 0)
                _unitOfWork.Factor.Remove(tblFactor);
            else
                tblFactor.TotalPrice -= (proc.Price - proc.OffPrice) * factProc.Count;

            try
            {
                await _unitOfWork.SaveAsync();
                return new JsonResult { Data = new Common.Result(Common.ResultMessage.OK) };
            }
            catch (Exception)
            {
                return new JsonResult { Data = new Common.Result(Common.ResultMessage.InternallServerError) };
            }
            #endregion
        }

        [HttpPost]
        public async Task<JsonResult> ChangeCount(int procId = 0, int count = 1)
        {
            if (procId == 0) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #region Get Product
            var proc = await _unitOfWork.Product.SingleByIdAsync(procId);
            if (proc == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            #region Get Customer ID
            var info = Function.GetCustInfo(Request);
            var hashToken = Function.GenerateHash(info.Token);
            custId = await Task.Run(() => _unitOfWork.Customer.Single(item => item.Token == hashToken)?.ID);
            if (custId == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            #region Get Customer Factor
            tblFactor = await _unitOfWork.Factor.SingleAsync(item =>
                item.CustID == custId && item.Status == (byte)Factor.FactStatus.InBascket);
            #endregion

            #region change factor item count
            var factProc = tblFactor.Tbl_FactProc.SingleOrDefault(item => item.ProcID == proc.ID);
            if (factProc == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };

            long finalPrice = (proc.Price - proc.OffPrice) * factProc.Count,
                 newPrice = (proc.Price - proc.OffPrice) * count;
            factProc.Count = (byte)count;
            tblFactor.TotalPrice -= finalPrice;
            tblFactor.TotalPrice += newPrice;

            try
            {
                await _unitOfWork.SaveAsync();
                return new JsonResult { Data = new Common.Result(Common.ResultMessage.OK) };
            }
            catch (Exception)
            {
                return new JsonResult { Data = new Common.Result(Common.ResultMessage.InternallServerError) };
            }
            #endregion
        }
        #endregion

        #region shopping
        [HttpGet]
        public async Task<ActionResult> Shopping()
        {
            #region Get Customer ID
            var info = Function.GetCustInfo(Request);
            var hashToken = Function.GenerateHash(info.Token);
            var tblCustomer = await _unitOfWork.Customer.SingleAsync(item => item.Token == hashToken);
            if (tblCustomer == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            #region check info
            if (tblCustomer.Family == null || tblCustomer.NatCode == null)
                return Redirect("/Account/Profile?Shopping");
            #endregion

            #region check address
            if (tblCustomer.Tbl_CustAddress.Count == 0)
                return Redirect("/Account/Address?Shopping");
            #endregion

            #region find order detail
            tblFactor = tblCustomer.Tbl_Factor.SingleOrDefault(item =>
                item.Status == (byte)Factor.FactStatus.InBascket);
            if (tblFactor == null)
            {
                ViewBag.Message = "سبد خرید شما خالی است!";
                return Redirect("/");
            }
            var orderDetail = tblFactor.Adapt<Factor.ViewOrderDetail>();
            return View(orderDetail);
            #endregion
        }
        #endregion
    }
}