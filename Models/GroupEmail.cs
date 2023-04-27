﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ChatManager.Models
{
    public class GroupEmail
    {
        public List<int> SelectedUsers { get; set; }

        [Display(Name = "Sujet"), Required(ErrorMessage = "Obligatoire")]
        public string Subject { get; set; }

        [Display(Name = "Message"), Required(ErrorMessage = "Obligatoire")]
        public string Message { get; set; }
        
        public void Send()
        {
            if (SelectedUsers != null)
            {
                foreach (int userId in SelectedUsers)
                {
                    User user = DB.Users.Get(userId);
                    string personalizedMessage = Message.Replace("[Nom]", user.GetFullName(true)).Replace("\r\n", @"<br>");
                    SMTP.SendEmail(user.GetFullName(), user.Email, Subject, personalizedMessage);
                }
            }
        }
    }
}