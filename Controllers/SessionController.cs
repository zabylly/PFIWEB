using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatManager.Controllers
{
    public class SessionController : Controller
    {
        public ActionResult End(string message)
        {
            if (Session["currentLoginId"] != null)
                DB.Users.UpdateLogout((int)Session["currentLoginId"]);
            OnlineUsers.RemoveSessionUser();
            return RedirectToAction("Login", "Accounts", new { message });
        }
    }
}