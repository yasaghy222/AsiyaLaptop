using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Src.Models.ViewData.Table.Customer;

namespace Src.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variable
        Tbl_Customer Customer;
        #endregion

        [HttpGet, PublicAction]
        public ActionResult Index() => View(new ViewAccountVar());

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
        async Task<bool> SetToken(Tbl_Customer customer)
        {
            customer.Token = Function.GenerateNewToken();
            CustomerInfo info = customer.Adapt<CustomerInfo>();
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
        [ValidateAntiForgeryToken, ValidateModel]
        public async Task<ActionResult> Index(ViewAccountVar accountVar)
        {
            if (accountVar.Name == null)
            {
                #region login
                string hashPass = Function.GenerateHash(accountVar.Pass);
                Customer = await _unitOfWork.Customer.SingleAsync(item =>
                                                                         item.Phone == accountVar.Phone &&
                                                                         item.Pass == hashPass);
                if (Customer != null)
                {
                    string message = await IsValid(Customer);
                    if (message == Common.ResultMessage.OK)
                    {
                        return Redirect("/");
                    }
                    else
                    {
                        ViewBag.Result = new Common.Result
                        {
                            Message = message
                        };
                        return View();
                    }
                }
                else
                {
                    ViewBag.Result = new Common.Result
                    {
                        Message = "شماره موبایل یا رمزعبور اشتباه است"
                    };
                    return View();
                }
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
                Customer = await _unitOfWork.Customer.SingleAsync(item => item.Token == hashToken);
                if (Customer != null)
                {
                    Customer.Token = null;
                    await _unitOfWork.SaveAsync();
                    try
                    {
                        ClearCookie();
                        ViewBag.RedirectPath = "/";
                        return View();
                    }
                    catch (Exception)
                    {
                        ViewBag.Result = new Common.Result
                        {
                            Message = Common.ResultMessage.InternallServerError
                        };
                        return View();
                    }
                }
                else
                {
                    ClearCookie();
                    ViewBag.RedirectPath = "/";
                    return View();
                }
                #endregion
            }
            else
            {
                ClearCookie();
                ViewBag.RedirectPath = "/";
                return View();
            }
            #endregion
        }
        #endregion
    }
}