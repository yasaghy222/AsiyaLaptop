using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Src.App_Start.FilterConfig;
using static Src.Models.ViewData.Table.Customer;

namespace Src.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variable
        Tbl_Customer tblCustomer;
        #endregion

        [HttpGet, PublicAction]
        public ActionResult Index() => View(new ViewAccountVar());

        [HttpGet]
        public async Task<ActionResult> Profile()
        {
            #region Get Customer
            var info = Function.GetCustInfo(Request);
            var hashToken = Function.GenerateHash(info.Token);
            tblCustomer = await _unitOfWork.Customer.SingleAsync(item => item.Token == hashToken);
            if (tblCustomer == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
            #endregion

            return View(tblCustomer.Adapt<ViewCustomer>());
        }

        #region change profile info
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Profile(ViewCustomer model)
        {
            #region update info
            tblCustomer = await _unitOfWork.Customer.SingleByIdAsync(model.ID);
            tblCustomer.Name = model.Name;
            tblCustomer.Family = model.Family;
            tblCustomer.Email = model.Email;
            tblCustomer.Phone = model.Phone;
            tblCustomer.NatCode = model.NatCode;
            tblCustomer.IP = Function.GetIP();
            tblCustomer.IsInNewsletter = model.IsInNewsletter;
            #endregion
            try
            {
                await _unitOfWork.SaveAsync();
                UpdateCookie(tblCustomer.Adapt<CustomerInfo>());
                string query = Request.Url.Query,
                       redirect = query != "" ? $"/{query.Substring(1, query.Length-1)}" : "/";
                return Redirect(redirect);
            }
            catch (Exception)
            {
                ViewBag.Message = Common.ResultMessage.InternallServerError;
                #region Get Customer
                var info = Function.GetCustInfo(Request);
                var hashToken = Function.GenerateHash(info.Token);
                tblCustomer = await _unitOfWork.Customer.SingleAsync(item => item.Token == hashToken);
                if (tblCustomer == null) return new JsonResult { Data = new Common.Result(Common.ResultMessage.NotFound) };
                #endregion
                return View(tblCustomer.Adapt<ViewCustomer>());
            }
        }
        #endregion

        [HttpGet, PublicAction]
        public ActionResult ResetPass() => View();

        #region authorize functions
        void SetCookie(CustomerInfo info)
        {
            ClearCookie();
            HttpCookie cookie = new HttpCookie("ALCustInfo")
            {
                Expires = DateTime.Now.AddMonths(3),
                HttpOnly = true
            };
            cookie.Values.Add("Token", info.Token);
            cookie.Values.Add("Name", info.Name);
            cookie.Values.Add("Family", info.Family);
            HttpContext.Response.SetCookie(cookie);
        }
        void UpdateCookie(CustomerInfo info)
        {
            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("ALCustInfo"))
            {
                var cookie = ControllerContext.HttpContext.Request.Cookies["ALCustInfo"];
                cookie.Values["Name"] = info.Name;
                cookie.Values["Family"] = info.Family;
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
        }
        async Task<bool> SetToken(Tbl_Customer customer)
        {
            customer.Token = Function.GenerateNewToken();
            var info = customer.Adapt<CustomerInfo>();
            customer.Token = Function.GenerateHash(customer.Token);
            await _unitOfWork.SaveAsync();
            try
            {
                SetCookie(info);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        async Task<string> IsValid(Tbl_Customer customer)
        {
            #region check status
            if (customer.Status)
            {
                return await SetToken(customer) ? Common.ResultMessage.OK : Common.ResultMessage.InternallServerError;
            }
            else
            {
                return Common.ResultMessage.AccountIsBlock;
            }
            #endregion
        }
        #endregion

        #region login/register
        [HttpPost, PublicAction]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ViewAccountVar accountVar)
        {
            if (accountVar.Name == null)
            {
                #region login
                var hashPass = Function.GenerateHash(accountVar.Pass);
                tblCustomer = await _unitOfWork.Customer.SingleAsync(item =>
                                                                         item.Phone == accountVar.Phone &&
                                                                         item.Pass == hashPass);
                if (tblCustomer != null)
                {
                    var message = await IsValid(tblCustomer);
                    if (message == Common.ResultMessage.OK)
                        return Redirect("/");

                    ViewBag.Result = new Common.Result(message);
                    return View();
                }

                ViewBag.Result = new Common.Result("شماره موبایل یا رمزعبور اشتباه است");
                return View();
                #endregion
            }
            else
            {
                #region register
                return View();
                #endregion
            }
        }
        #endregion

        #region logout
        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            string token = ControllerContext.HttpContext.Request.Cookies["ALCustInfo"]?["Token"].ToString();
            #region check token not null
            if (token != null)
            {
                string hashToken = Function.GenerateHash(token);
                #region get customer with token
                tblCustomer = await _unitOfWork.Customer.SingleAsync(item => item.Token == hashToken);
                if (tblCustomer != null)
                {
                    tblCustomer.Token = null;
                    await _unitOfWork.SaveAsync();
                    try
                    {
                        ClearCookie();
                    }
                    catch (Exception)
                    {
                        ViewBag.Result = new Common.Result
                        {
                            Message = Common.ResultMessage.InternallServerError
                        };
                    }
                }
                else
                {
                    ClearCookie();
                }
                #endregion
            }
            else
            {
                ClearCookie();
            }

            return Redirect("/");
            #endregion
        }
        #endregion
    }
}