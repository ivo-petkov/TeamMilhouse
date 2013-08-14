using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WebChat.Data;
using WebChat.Model;
using WebChat.Services.Controllers;
using WebChat.Services.Models;

namespace WebChat.Services.Persisters
{
    public class MessageDataPersister
    {
        public static IEnumerable<CollectedMessages> GetAllMessagesWithUser(int currUser, string otherUser)
        {
            using (var context = new WebChatContext())
            {

                var messages = context.Messages.Where(x => (x.Sender.UserId == currUser &&
                          x.Receiver.Username == otherUser) ||
                          (x.Sender.Username == otherUser
                          && x.Receiver.UserId == currUser)).OrderBy(x => x.SentTime).Select(x => new CollectedMessages()
                          {
                              Sender = x.Sender.Username,
                              Content = x.Content,
                              Time = x.SentTime
                          }).ToList();

                return messages;
            }
        }

        public static void ClearNotificationsWithUser(int currUser, string otherUser)
        {
            using (var context = new WebChatContext())
            {
                var messages = context.Messages.Where(x => x.Sender.Username == otherUser
                          && x.Receiver.UserId == currUser).ToList();
                foreach (var message in messages)
                {
                    message.Status = true;
                }
                context.SaveChanges();
            }
        }
    }
}