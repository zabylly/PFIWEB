using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public int IdFriend { get; set; }
        public int FriendStatus { get; set; }
        public bool DeniedFriend { get; set; }

        [JsonIgnore]
        public User Friend
        {
            get
            {
                return DB.Users.Get(IdFriend);
            }
        }
    }
}