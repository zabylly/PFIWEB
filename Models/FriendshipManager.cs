using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class FriendshipManager : Repository<Friendship>
    {
        //missing blocked, mais doit être filtrer a travers User pas Friendship
        public IEnumerable<Friendship> SortedUsersByCategory(int userId, bool askFriend, bool askedFriend, bool deniedFriend)
        {
            return ToList().Where(u => u.userId == userId).Where(u => u.askFriend = askFriend).Where(u => u.askedFriend = askedFriend)
                .Where(u => u.deniedFriend = deniedFriend);
        }
    }
}