﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ChatManager.Models
{
    public class LoginCredential
    {
        [Display(Name = "Courriel"), EmailAddress(ErrorMessage = "Invalide"), Required(ErrorMessage = "Obligatoire")]
        [System.Web.Mvc.Remote("EmailExist", "Accounts", HttpMethod = "POST", ErrorMessage = "Courriel introuvable.")]
        public string Email { get; set; }

        [Display(Name = "Mot de passe"), Required(ErrorMessage = "Obligatoire")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [JsonIgnore]
        public int TimeZoneOffset { get; set; }
    }
}