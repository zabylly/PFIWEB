using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class FriendshipRepository : Repository<Friendship>
    {
        //missing blocked, mais doit être filtrer a travers User pas Friendship
        public IEnumerable<Friendship> SortedFriendshipByCategory(int userId, bool deniedFriend, params int[] friend)
        {
            return ToList().Where(u => u.Id == userId).Where(u => u.DeniedFriend = deniedFriend);
        }
    }
}