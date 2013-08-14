using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChat.Model
{
    public class User
    {
        //private ICollection<Message> sentMessages;
        //private ICollection<Message> receivedMessages;
        //private ICollection<File> sentFiles;
        //private ICollection<File> receivedFiles;

        public int UserId { get; set; }

        public string Username { get; set; }

        public string AuthCode { get; set; }

        public string Avatar { get; set; }

        //public User()
        //{
        //    this.sentMessages = new HashSet<Message>();
        //    this.receivedMessages = new HashSet<Message>();
        //    this.sentFiles = new HashSet<File>();
        //    this.receivedFiles = new HashSet<File>();
        //}


        //public virtual ICollection<Message> SentMessages
        //{
        //    get { return this.sentMessages; }
        //    set { this.sentMessages = value; }
        //}

        //public virtual ICollection<Message> ReceivedMessages
        //{
        //    get { return this.receivedMessages; }
        //    set { this.receivedMessages = value; }
        //}

        //public virtual ICollection<File> SentFiles
        //{
        //    get { return this.sentFiles; }
        //    set { this.sentFiles = value; }
        //}

        //public virtual ICollection<File> ReceivedFiles
        //{
        //    get { return this.receivedFiles; }
        //    set { this.receivedFiles = value; }
        //}

    }
}
