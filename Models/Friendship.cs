using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Friendship
    {
        public int userId { get; set; }
        public int friendId { get; set; }
        public bool askFriend { get; set; }
        public bool askedFriend { get; set; }
        public bool deniedFriend { get; set; }
    }
}