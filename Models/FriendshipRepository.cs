﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Services.Description;

namespace ChatManager.Models
{
    public class FriendshipRepository : Repository<Friendship>
    {
        //missing blocked, mais doit être filtrer a travers User pas Friendship
        public IEnumerable<Friendship> SortedFriendshipByCategory(int userId, bool showAccountBlocked, string name, params int[] relationToShow)
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
                friendshipsToShow = allFriendship;
            }
            if (!showAccountBlocked)
            {
                friendshipsToShow = friendshipsToShow.Where(u => u.Friend.Blocked == false);
            }
            if (name.Length > 0)
            {
                name = name.ToLower();
                friendshipsToShow = friendshipsToShow.Where(u => u.Friend.FirstName.ToLower().Contains(name) || u.Friend.LastName.ToLower().Contains(name));
            }
            //friendshipsToShow = friendshipsToShow.Where(u => u.FriendStatus != Friendship.Accepted);
            return friendshipsToShow.OrderBy(u => u.FriendStatus);//.Where(u => u.DeniedFriend == deniedFriend);
        }
        public List<Friendship> GetListFriendshipWithNullRelation(int userId)
        {
            IEnumerable<Friendship> friendships = ToList().Where(u => u.IdUser == userId);
            List<Friendship> friendshipsComplete = new List<Friendship>(friendships);
            bool IsInRelation(User user)
            {

                foreach (Friendship friendship in friendships)
                {
                    if (friendship.IdFriend == user.Id) return true;
                }
                return false;
            }



            foreach (User friend in DB.Users.ToList())
            {
                if (!IsInRelation(friend) && friend.Verified && friend.Id != userId)
                {
                    Friendship fship = new Friendship();
                    fship.IdUser = userId;
                    fship.IdFriend = friend.Id;
                    fship.FriendStatus = 0;
                    friendshipsComplete.Add(fship);
                }
            }
            return friendshipsComplete;
        }
        public IEnumerable<Friendship> GetListFriends(int userId)
        {
            return ToList().Where(u => u.IdUser == userId && u.FriendStatus == Friendship.Accepted);
        }
        public Friendship SendInvitation(int id, int idFriend)
        {
            try
            {
                Friendship demandeAmis = new Friendship();
                demandeAmis.IdUser = id;
                demandeAmis.IdFriend = idFriend;
                Friendship friendRelation = FindFriendRelation(demandeAmis);
                if (friendRelation != null && friendRelation.FriendStatus == Friendship.RequestSend)//si l'autre amis ta envoyer une requete en meme temps
                {
                    return AcceptFriendRequest(id, idFriend);
                }
                if(friendRelation != null && friendRelation.FriendStatus == Friendship.DeclineByThem)
                {
                    return ReinviteBlockedFriend(id, idFriend);
                }
                demandeAmis.FriendStatus = Friendship.RequestSend;
                demandeAmis.Id = base.Add(demandeAmis);
                Friendship receveurDemande = new Friendship();
                receveurDemande.IdUser = demandeAmis.IdFriend;
                receveurDemande.IdFriend = demandeAmis.IdUser;
                receveurDemande.FriendStatus = Friendship.RequestReceved;
                receveurDemande.Id = base.Add(receveurDemande);
                OnlineUsers.AddNotification(idFriend, "Vous avez recu une demande d'amis");
                return demandeAmis;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Frienship Request Failed : Message - {ex.Message}");
            }
            return null;
        }
        public Friendship AcceptFriendRequest(int id, int idFriend)
        {
            try
            {
                Friendship friendship = FindRelationById(id, idFriend);
                Friendship friendRelation = FindFriendRelation(friendship);
                friendRelation.FriendStatus = Friendship.Accepted;
                base.Update(friendRelation);
                friendship.FriendStatus = Friendship.Accepted;
                base.Update(friendship);
                return friendship;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Frienship Request Failed : Message - {ex.Message}");
            }
            return null;
        }
        public Friendship DeclineFriendRequest(int id,int idFriend)
        {
            try
            {
                Friendship friendship = FindRelationById(id, idFriend);
                Friendship friendRelation = FindFriendRelation(friendship);
                friendRelation.FriendStatus = Friendship.DeclineByThem;
                base.Update(friendRelation);
                friendship.FriendStatus = Friendship.DeclineByYou;
                base.Update(friendship);
                return friendship;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Frienship Request Failed : Message - {ex.Message}");
            }
            return null;

        }
        public void RemoveRelation(int id, int idFriend)
        {
            try
            {
                Friendship friendship = FindRelationById(id, idFriend);
                Friendship friendRelation = FindFriendRelation(friendship);
                base.Delete(friendship.Id);
                base.Delete(friendRelation.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Frienship Request Failed : Message - {ex.Message}");
            }

        }

        public Friendship ReinviteBlockedFriend(int id,int idFriend)
        {
            try
            {
                Friendship friendship = FindRelationById(id, idFriend);
                Friendship friendRelation = FindFriendRelation(friendship);
                friendRelation.FriendStatus = Friendship.RequestReceved;
                base.Update(friendRelation);
                friendship.FriendStatus = Friendship.RequestSend;
                base.Update(friendship);
                return friendship;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Frienship Request Failed : Message - {ex.Message}");
            }
            return null;

        }
        public Friendship FindFriendRelation(Friendship friendship)
        {
            IEnumerable<Friendship> FriendTmp = ToList().Where(u => u.IdUser == friendship.IdFriend && u.IdFriend == friendship.IdUser);
            if (FriendTmp.Count() > 0) return FriendTmp.First();
            else return null;
        }
        public Friendship FindRelationById(int id, int idFriend)
        {
            IEnumerable<Friendship> FriendTmp = ToList().Where(u => u.IdUser == id && u.IdFriend == idFriend);
            if (FriendTmp.Count() > 0) return FriendTmp.First();
            else return null;
        }
    }
}