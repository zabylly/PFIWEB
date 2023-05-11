using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Friendship : IEquatable<int>
    {
        public const int Nothing = 0;
        public const int RequestSend = 1;
        public const int RequestReceved = 2;
        public const int Accepted = 3;
        public const int DeclineByYou = 4;
        public const int DeclineByThem = 5;
        public Friendship()
        {
            IdUser = 0;
            IdFriend = 0;
            FriendStatus = 0;
        }

        public int Id { get; set; }
        public int IdUser { get; set; }
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