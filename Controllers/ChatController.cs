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
        int id = OnlineUsers.GetSessionUser().Id;
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
            if (forceRefresh || OnlineUsers.HasChanged() || DB.Friendships.HasChanged)
            {
                return PartialView(DB.Friendships.GetListFriends(OnlineUsers.GetSessionUser().Id));
            }
            return null;
        }
        public ActionResult GetChatLog(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || DB.Message.HasChanged)
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
        public void SendText(int idRecever,string text) 
        {
           DB.Message.SaveMessage(id,idRecever,text);
        }
    }
}