using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChatManager.Models
{
    public class Login
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime LogoutDate { get; set; }
        public string IpAddress { get; set; }
        public string City { get; set; } = "";
        public string RegionName { get; set; } = "";
        public string CountryCode { get; set; } = "";

        [JsonIgnore]
        public User User
        {
            get
            {
                return DB.Users.Get(UserId);
            }
        }
    }
}
