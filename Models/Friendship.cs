using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Friendship : IEquatable<int>
    {
        public Friendship(int id, int idFriend, int friendStatus)
        {
            Id = id;
            IdFriend = idFriend;
            FriendStatus = friendStatus;
        }

        public int Id { get; set; }
        public int IdFriend { get; set; }
        public int FriendStatus { get; set; }
        //0 rien
        //1 requete envoye
        //2 requete recus
        //3 amis
        //4 décliné par vous
        //5 décliné par l'utilisateur
        [JsonIgnore]
        public User Friend
        {
            get
            {
                return DB.Users.Get(IdFriend);
            }
        }

        public bool Equals(int id)
        {
            return IdFriend.Equals(id);
        }
    }
}