using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using WebChat.Model;

namespace WebChat.Data
{
    public class WebChatContext : DbContext
    {
        public WebChatContext()
            : base("WebChat")
        {
        }

        public DbSet<File> Files { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

    }
}
