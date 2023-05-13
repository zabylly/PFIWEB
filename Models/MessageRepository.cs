using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class MessageRepository : Repository<Message>
    {
        public IEnumerable<Message> GetMessageUser(int id)
        {
            return ToList().Where(u => u.IdSender == id || u.IdRecever == id ).OrderBy(u=>u.DateSent);      
        }

        public string SaveMessage(int idSender, int idRecever, string text)
        {
            try
            {
                Message newMessage = new Message();
                newMessage.IdSender = idSender;
                newMessage.IdRecever = idRecever;
                newMessage.Text = text;
                base.Add(newMessage);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" Message send failed : Message - {ex.Message}");
            }
            return null;
        }

    }
}