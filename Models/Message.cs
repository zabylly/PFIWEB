using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Message
    {
        public Message()
        {
            Id = 0;
            IdSender = 0;
            IdRecever = 0;
            DateSent = DateTime.Now;
            Text = string.Empty;
        }
        public int Id { get; set; }
        public int IdSender { get; set; }
        public int IdRecever { get; set; }
        public string Text { get; set; }
        public DateTime DateSent { get; set; }
    }
}