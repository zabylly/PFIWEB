using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace ChatManager.Models
{
    public class FriendshipRepository : Repository<Friendship>
    {
        //missing blocked, mais doit être filtrer a travers User pas Friendship
        public IEnumerable<Friendship> SortedFriendshipByCategory(int userId, bool showAccountBlocked, params int[] relationToShow)
        {
            IEnumerable<Friendship> allFriendship = GetListFriendshipWithNullRelation(userId);
            IEnumerable<Friendship> friendshipsToShow = new List<Friendship>();


            if (relationToShow != null)
            {
                foreach (int relation in relationToShow)
                {
                    friendshipsToShow = friendshipsToShow.Concat(allFriendship.Where(u => u.FriendStatus == relation));
                }
            }
            else
            {
                friendshipsToShow= allFriendship;
            }
            if(!showAccountBlocked)
            {
                friendshipsToShow = friendshipsToShow.Where(u => u.Friend.Blocked == true);
            }
                //friendshipsToShow = friendshipsToShow.Where(u => u.FriendStatus != Friendship.Accepted);
            return friendshipsToShow;//.Where(u => u.DeniedFriend == deniedFriend);
        }
        public List<Friendship> GetListFriendshipWithNullRelation(int userId)
        {
            IEnumerable<Friendship> friendships = ToList().Where(u => u.Id == userId);
            List<Friendship> friendshipsComplete = new List<Friendship>(friendships);

            foreach (User amis in DB.Users.ToList())
            {
                foreach (Friendship friendship in friendships)
                {
                    if (!friendship.Equals(amis.Id) && amis.Id != userId)
                    {
                        Friendship fship = new Friendship();
                        fship.Id = userId;
                        fship.IdFriend = amis.Id;
                        fship.FriendStatus = 0;
                        friendshipsComplete.Add(fship);
                    }
                }
            }
            return friendshipsComplete;
        }
        public Friendship EnvoyerDemandeAmis(Friendship demandeAmis)
        {
            try
            {
                OnlineUsers.SetHasChanged();
                Friendship friendRelation = FindFriendRelation(demandeAmis);
                if (friendRelation != null && friendRelation.FriendStatus == Friendship.RequestSend)//si l'autre amis ta envoyer une requete en meme temps
                {
                    
                }
                demandeAmis.FriendStatus = Friendship.RequestSend;
                demandeAmis.Id = base.Add(demandeAmis);
                Friendship receveurDemande = new Friendship();
                receveurDemande.Id= demandeAmis.IdFriend;
                receveurDemande.IdFriend = demandeAmis.Id;
                receveurDemande.FriendStatus = Friendship.RequestReceved;
                base.Add(receveurDemande);
                return demandeAmis;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Frienship Request Failed : Message - {ex.Message}");
            }
            return null;
        }
        //public Friendship AccepterDemandeAmis(Friendship friendship)
        //{
        //    OnlineUsers.SetHasChanged();
        //    Friendship friendRelation = FindFriendRelation(demandeAmis);
        //}
        public Friendship FindFriendRelation(Friendship friendship)
        {
            return ToList().Where(u => u.Id == friendship.IdFriend && u.IdFriend == friendship.Id).First();

        }
    }
}