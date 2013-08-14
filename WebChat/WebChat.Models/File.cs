using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChat.Model
{
    public class File
    {
        public int FileId { get; set; }

        public string Link { get; set; }

        public DateTime UploadTime { get; set; }

         
        //public int SenderId { get; set; }

        //[ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        //public int ReceiverId { get; set; }

        //[ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        public bool Status { get; set; }
    }
}
