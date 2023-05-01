using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatManager.Controllers
{
    public class FriendshipsController : Controller
    {
        // GET: Friendships
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUsersList(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged())
            {
                return PartialView(DB.Users.SortedUsers());
            }
            return null;
        }
    }
}