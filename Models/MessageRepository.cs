using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class MessageRepository : Repository<Message>
    {
        public IEnumerable<Message> GetMessageChat(int id,int idFriend)
        {
            return ToList().Where(u => u.IdSender == id && u.IdRecever == idFriend || u.IdSender == idFriend && u.IdRecever == id).OrderBy(u=>u.DateSent);      
        }

        public void SaveMessage(int idSender, int idRecever, string text)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    Message newMessage = new Message();
                    newMessage.IdSender = idSender;
                    newMessage.IdRecever = idRecever;
                    newMessage.Text = text;
                    base.Add(newMessage);
                    OnlineUsers.AddNotification(idRecever, "Vous avez recu un message de " + DB.Users.FindUser(idSender).GetFullName());
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" Message send failed : Message - {ex.Message}");
            }
        }

        public void ChangeMessage(int idMessage, string text)
        {
            try
            {

                DB.Message.Get(idMessage).Text = text;

                if (string.IsNullOrWhiteSpace(text))
                    DeleteMessage(idMessage);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" Message send failed : Message - {ex.Message}");
            }
        }
        public void DeleteMessage(int idMessage)
        {
            try
            {
                Delete(idMessage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" Message send failed : Message - {ex.Message}");
            }
        }

    }
}