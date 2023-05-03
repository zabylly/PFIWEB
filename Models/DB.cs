using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Hosting;
using System.Web.Services.Description;
using System.Web.UI.WebControls;

namespace ChatManager.Models
{
    public class DB
    {
        #region singleton setup
        private static readonly DB instance = new DB();
        public static DB Instance
        {
            get { return instance; }
        }
        #endregion
        #region Repositories
        public static Repository<Friendship> Friendships { get; set; }
        public static Repository<Gender> Genders { get; set; }
        public static Repository<UserType> UserTypes { get; set; }
        public static Repository<UnverifiedEmail> UnverifiedEmails { get; set; }
        public static Repository<ResetPasswordCommand> ResetPasswordCommands { get; set; }
        public static Repository<Login> Logins { get; set; }
        public static UsersRepository Users { get; set; }
        #endregion
        #region initialization
        public DB()
        {
            Friendships = new Repository<Friendship>();
            Genders = new Repository<Gender>();
            UserTypes = new Repository<UserType>();
            UnverifiedEmails = new Repository<UnverifiedEmail>();
            ResetPasswordCommands = new Repository<ResetPasswordCommand>();
            Logins = new Repository<Login>();
            Users = new UsersRepository();
            InitRepositories(this);
        }
        private static void InitRepositories(DB db)
        {
            string serverPath = HostingEnvironment.MapPath(@"~/App_Data/");
            PropertyInfo[] myPropertyInfo = db.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in myPropertyInfo)
            {
                if (propertyInfo.PropertyType.Name.Contains("Repository"))
                {
                    MethodInfo method = propertyInfo.PropertyType.GetMethod("Init");
                    method.Invoke(propertyInfo.GetValue(db), new object[] { serverPath + propertyInfo.Name + ".json" });
                }
            }
        }
        #endregion
    }
}