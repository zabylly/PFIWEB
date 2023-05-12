using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static System.Collections.Specialized.BitVector32;

namespace ChatManager.Controllers
{
    public class FriendshipsController : Controller
    {
        // GET: Friendships
        int id = OnlineUsers.GetSessionUser().Id;
        public ActionResult Index()
        {
            return View();
        }
        public void UpdateNameSearch(string search) 
        {
            Session["nameSearch"] = search;
        }

        public void UpdateSearch(string recherche)
        {
            Session["paramSearch"] = recherche;
        }

        public ActionResult GetFriendshipsList(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || new FriendshipRepository().HasChanged)
            {
                int[] paramInt = null;
                bool blocked = true;
                if (Session["paramSearch"] != null)
                {
                    string[] paramString = Session["paramSearch"].ToString().Split('_');
                    paramInt = new int[paramString.Length - 1];
                    for (int i = 0; i < paramString.Length - 1; i++)
                    {
                        paramInt[i] = int.Parse(paramString[i]);
                    }
                    blocked = paramString[paramString.Length - 1] == "1";
                }
                if (Session["nameSearch"] == null)
                    Session["nameSearch"] = "";
                return PartialView(DB.Friendships.SortedFriendshipByCategory(OnlineUsers.GetSessionUser().Id, blocked, Session["nameSearch"].ToString(), paramInt));
            }
            return null;
        }
        public void SendInvitation(int idFriend)
        {

            DB.Friendships.SendInvitation(id, idFriend);
            OnlineUsers.AddNotification(id, "Demande d'amis envoyé");
        }
        public void AccepteInvitation(int idFriend)
        {
            DB.Friendships.AcceptFriendRequest(id, idFriend);
            OnlineUsers.AddNotification(id, "Demande d'amis accepté");
        }
        public void EnleveRequeteAmitie(int idFriend)
        {
            DB.Friendships.RemoveRelation(id, idFriend);
            OnlineUsers.AddNotification(id, "Demande d'amis annulé");
        }
        public void RefuseAmitie(int idFriend)
        {
            DB.Friendships.DeclineFriendRequest(id, idFriend);
            OnlineUsers.AddNotification(id, "Demande d'amis refusé");
        }
        public void TermineAmitie(int idFriend)
        {
            DB.Friendships.DeclineFriendRequest(id, idFriend);
            OnlineUsers.AddNotification(id, "Amis retiré");
        }
    }
}