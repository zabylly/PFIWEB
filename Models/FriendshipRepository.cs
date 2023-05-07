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
            

            foreach (int relation in relationToShow)
            {
                friendshipsToShow = friendshipsToShow.Concat(allFriendship.Where(u => u.FriendStatus == relation));
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
                IEnumerable<Friendship> friendships = ToList().Where(u => u.Id == demandeAmis.IdFriend && u.IdFriend == demandeAmis.Id);//supposé etre vide
                if (friendships.Count() == 1 && friendships.First().FriendStatus == Friendship.RequestSend)//si l'autre amis ta envoyer une requete en meme temps
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

    }
}