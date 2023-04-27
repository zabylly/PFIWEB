using System.Web.Mvc;
using ChatManager.Models;

namespace ChatManager.Controllers
{
    public class CountryFlagController : Controller
    {
        public ActionResult Get(string countryCode)
        {
            return Json(Countries.FlagUrl(countryCode), JsonRequestBehavior.AllowGet);
        }
    }
}