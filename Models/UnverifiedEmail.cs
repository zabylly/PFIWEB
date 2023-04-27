using System;
using System.Collections.Generic;

namespace ChatManager.Models
{
    public class UnverifiedEmail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string VerificationCode { get; set; }
        public int UserId { get; set; }
    }
}
