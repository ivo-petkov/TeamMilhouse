using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChat.Model
{
    public class Message
    {
        public int MessageId { get; set; }

        public string Content { get; set; }

        public DateTime SentTime { get; set; }

         
        //public int SenderId { get; set; }

        //[ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

         
        //public int ReceiverId { get; set; }

        //[ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        public bool Status { get; set; }

    }
}
