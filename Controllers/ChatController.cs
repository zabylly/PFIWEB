﻿using ChatManager.Models;
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
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            return View();
        }
        public void UpdateChatLog(int idFriend)
        {
            Session["idFriendChat"] = idFriend;
        }
        [OnlineUsers.UserAccess]
        public ActionResult GetFriend(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || DB.Friendships.HasChanged)
            {
                return PartialView(DB.Friendships.GetListFriends(OnlineUsers.GetSessionUser().Id));
            }
            return null;
        }
        public ActionResult GetChatLog(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Friendships.HasChanged || DB.Message.HasChanged)
            {
                if (Session["idFriendChat"] != null)
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
        public void AddMessage(string message)
        {
            DB.Message.SaveMessage(OnlineUsers.GetSessionUser().Id, (int)Session["idFriendChat"], message);

        }
        public void ChangeMessage(int id, string message)
        {
            DB.Message.ChangeMessage(id, message);
        }
    }
}