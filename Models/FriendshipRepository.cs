using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class FriendshipRepository : Repository<Friendship>
    {
        //missing blocked, mais doit être filtrer a travers User pas Friendship
        public IEnumerable<Friendship> SortedFriendshipByCategory(int userId,bool noRelation,bool accepted, params int[] friend)
        {
            IEnumerable<Friendship> friendshipsToShow = GetListFriendshipWithNullRelation(userId);
            
            if(!noRelation)
            {
                friendshipsToShow = friendshipsToShow.Where(u => u.FriendStatus != Friendship.Nothing);
            }
            if(!accepted)
            {
                friendshipsToShow = friendshipsToShow.Where(u => u.FriendStatus != Friendship.Accepted);
            }
            return GetListFriendshipWithNullRelation(userId);//.Where(u => u.DeniedFriend == deniedFriend);
        }
        public List<Friendship> GetListFriendshipWithNullRelation(int userId)
        {
            IEnumerable<Friendship> friendships = ToList().Where(u => u.Id == userId);
            List<Friendship> friendshipsComplete = new List<Friendship>(friendships);

            foreach (User user in DB.Users.ToList())
            {
                foreach (Friendship friendship in friendships)
                {
                    if (!friendship.Equals(user.Id) && user.Id != userId)
                    {
                        Friendship fship = new Friendship();
                        fship.Id = userId;
                        fship.IdFriend = user.Id;
                        fship.FriendStatus = 0;
                        friendshipsComplete.Add(fship);
                    }
                }
            }
            return friendshipsComplete;
        }


    }
}