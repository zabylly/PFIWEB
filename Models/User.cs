using FileKeyReference;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ChatManager.Models
{
    public class User
    {
        public User()
        {
            Blocked = false;
            Verified = false;
            UserTypeId = 3;
            CreationDate = DateTime.Now;
            AcceptNotification = true;
        }
        public User Clone()
        {
            return JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(this));
        }
        #region Data Members
        public int Id { get; set; }
        [Display(Name = "Type usager")]
        public int UserTypeId { get; set; }
        public bool Verified { get; set; }
        public bool Blocked { get; set; }

        [Display(Name = "Prenom"), Required(ErrorMessage = "Obligatoire")]
        public string FirstName { get; set; }

        [Display(Name = "Nom"), Required(ErrorMessage = "Obligatoire")]
        public string LastName { get; set; }

        [Display(Name = "Genre")]
        public int GenderId { get; set; }

        [Display(Name = "Courriel"), EmailAddress(ErrorMessage = "Invalide"), Required(ErrorMessage = "Obligatoire")]
        [System.Web.Mvc.Remote("EmailAvailable", "Accounts", HttpMethod = "POST", AdditionalFields = "Id", ErrorMessage = "Ce courriel n'est pas disponible.")]
        public string Email { get; set; }

        public string Avatar { get; set; }

        [Display(Name = "Mot de passe"), Required(ErrorMessage = "Obligatoire")]
        [StringLength(50, ErrorMessage = "Le mot de passe doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [JsonIgnore]
        [Display(Name = "Confirmation")]
        [Compare("Email", ErrorMessage = "Le courriel et celui de confirmation ne correspondent pas.")]
        public string ConfirmEmail { get; set; }

        [JsonIgnore]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmation")]
        [Compare("Password", ErrorMessage = "Le mot de passe et celui de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Date de création")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }
        #endregion
        #region Avatar handling
        [JsonIgnore]
        [Display(Name = "Avatar")]
        public string AvatarImageData { get; set; }
        [JsonIgnore]
        private static ImageFileKeyReference AvatarReference =
            new ImageFileKeyReference(@"/Images_Data/User_Avatars/", @"no_avatar.png", false);
        public String GetAvatarURL()
        {
            return AvatarReference.GetURL(Avatar, false);
        }
        public void SaveAvatar()
        {
            Avatar = AvatarReference.Save(AvatarImageData, Avatar);
        }
        public void RemoveAvatar()
        {
            AvatarReference.Remove(Avatar);
        }
        #endregion
        #region View members
        [JsonIgnore]
        public bool AcceptNotification { get; set; }
        [JsonIgnore]
        public Gender Gender { get { return DB.Genders.Get(GenderId); } }
        [JsonIgnore]
        public UserType UserType { get { return DB.UserTypes.Get(UserTypeId); } }
        [JsonIgnore]
        public bool IsPowerUser { get { return UserTypeId <= 2 /* Admin = 1 , PowerUser = 2 */; } }
        [JsonIgnore]
        public bool IsAdmin { get { return UserTypeId == 1 /* Admin */; } }
        [JsonIgnore]
        public bool CRUD_Access { get { return IsPowerUser; } }
        public string GetFullName(bool showGender = false)
        {
            if (showGender)
            {
                if (Gender.Name != "Neutre")
                    return Gender.Name + " " + LastName;
            }
            return FirstName + " " + LastName;
        }
        #endregion
    }
}