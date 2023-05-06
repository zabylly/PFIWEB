using System;
using System.Collections.Generic;
using System.Linq;
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


    }
}