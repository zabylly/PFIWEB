using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatManager.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Index()
        {
            return View();
        }
        public void UpdateChatLog(int idFriend)
        {
            Session["idFriendChat"] = idFriend;
        }

        public ActionResult GetFriend(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || new FriendshipRepository().HasChanged)
            {
                return PartialView(DB.Friendships.GetListFriends(OnlineUsers.GetSessionUser().Id));
            }
            return null;
        }
        public ActionResult GetChatLog(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || new FriendshipRepository().HasChanged)
            {
                if (Request.Cookies["idFriendChat"] != null)
                {
                    ViewBag.Recipient = DB.Users.Get((int)Session["idFriendChat"]);
                    return PartialView(DB.Message.GetMessageChat(OnlineUsers.GetSessionUser().Id, (int)Session["idFriendChat"]));
                }
                    
                else
                {
                    ViewBag.Recipient = null;
                    return PartialView(null);
                }
            }
            return null;
        }
    }
}