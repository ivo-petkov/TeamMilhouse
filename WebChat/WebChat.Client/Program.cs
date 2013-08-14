using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChat.Model;
using WebChat.Data;

namespace WebChat.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new WebChatContext();
            db.Database.CreateIfNotExists();

            User user = new User
            {
                Username = "user1",

            };

            db.Users.Add(user);
            db.SaveChanges();
        }
    }
}
