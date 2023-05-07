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

        public ActionResult GetFriendshipsList(string recherche, bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged())
            {
                int[] paramInt = null;
                if (recherche != null)
                {
                    string[] paramString = recherche.Split('_');
                    paramInt = new int[paramString.Length - 1];
                    for (int i = 0; i < paramString.Length - 1; i++)
                    {
                        paramInt[i] = int.Parse(paramString[i]);
                    }
                }
                return PartialView(DB.Friendships.SortedFriendshipByCategory(1/*besoin user id*/, true, paramInt));
            }
            return null;
        }
    }
}