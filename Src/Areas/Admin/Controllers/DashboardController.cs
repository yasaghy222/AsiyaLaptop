using System.Web.Mvc;

namespace Src.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        // GET: Admin/Dashboard
        public ActionResult Index() => View();

        public ActionResult Dashboard() => View();
    }
}