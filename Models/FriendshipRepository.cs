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
        public IEnumerable<Friendship> SortedFriendshipByCategory(int userId, bool showAccountBlocked,string name,params int[] relationToShow)
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
                friendshipsToShow = friendshipsToShow.Where(u => u.Friend.Blocked == false);
            }
            if(name.Length> 0)
            {
                name = name.ToLower();
                friendshipsToShow = friendshipsToShow.Where(u => u.Friend.FirstName.ToLower().Contains(name) || u.Friend.LastName.ToLower().Contains(name));
            }
                //friendshipsToShow = friendshipsToShow.Where(u => u.FriendStatus != Friendship.Accepted);
            return friendshipsToShow.OrderBy(u => u.FriendStatus);//.Where(u => u.DeniedFriend == deniedFriend);
        }
        public List<Friendship> GetListFriendshipWithNullRelation(int userId)
        {
            IEnumerable<Friendship> friendships = ToList().Where(u => u.Id == userId);
            List<Friendship> friendshipsComplete = new List<Friendship>(friendships);
            bool IsInRelation(User user)
            {

                foreach (Friendship friendship in friendships)
                {
                    if(friendship.IdFriend == user.Id) return true;
                }
                return false;
            }

 

            foreach (User friend in DB.Users.ToList())
            {
                    if (!IsInRelation(friend) && friend.Verified && friend.Id != userId)
                    {
                        Friendship fship = new Friendship();
                        fship.Id = userId;
                        fship.IdFriend = friend.Id;
                        fship.FriendStatus = 0;
                        friendshipsComplete.Add(fship);
                    }
            }
            return friendshipsComplete;
        }
        public Friendship SendInvitation(int id, int idFriend)
        {
            try
            {
                Friendship demandeAmis = new Friendship();
                demandeAmis.Id = id;
                demandeAmis.IdFriend = idFriend;
                OnlineUsers.SetHasChanged();
                Friendship friendRelation = FindFriendRelation(demandeAmis);
                if (friendRelation != null && friendRelation.FriendStatus == Friendship.RequestSend)//si l'autre amis ta envoyer une requete en meme temps
                {
                    AcceptFriendRequest(demandeAmis);
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
        public Friendship AcceptFriendRequest(Friendship friendship)
        {
            Friendship friendRelation = FindFriendRelation(friendship);
            friendRelation.FriendStatus = Friendship.Accepted;
            base.Update(friendRelation);
            friendship.FriendStatus = Friendship.Accepted;
            base.Update(friendship);
            return friendship;
        }
        public Friendship DeclineFriendRequest(Friendship friendship)
        {
            Friendship friendRelation = FindFriendRelation(friendship);
            friendRelation.FriendStatus = Friendship.DeclineByThem;
            base.Update(friendRelation);
            friendship.FriendStatus = Friendship.DeclineByYou;
            base.Update(friendship);
            return friendship;

        }
        public Friendship ReinviteBlockedFriend(Friendship friendship)
        {
            Friendship friendRelation = FindFriendRelation(friendship);
            friendRelation.FriendStatus = Friendship.RequestReceved;
            base.Update(friendRelation);
            friendship.FriendStatus = Friendship.RequestSend;
            base.Update(friendship);
            return friendship;

        }
        //public Friendship RemoveFriendRequest(Friendship friendship)
        //{
        //    OnlineUsers.SetHasChanged();
        //    Friendship friendRelation = FindFriendRelation(friendship);
        //    base.Delete(friendRelation.Id);
        //    base.Delete(friendship.Id);
        //    return friendship;
        //}
        public Friendship FindFriendRelation(Friendship friendship)
        {
            return ToList().Where(u => u.Id == friendship.IdFriend && u.IdFriend == friendship.Id).First();
        }
    }
}