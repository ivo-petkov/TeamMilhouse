using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebChat.Services.Models
{
    [DataContract]
    public class SendMessageModel
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "to")]
        public string Receiver { get; set; }
    }


    [DataContract]
    public class GetMessagesModel
    {
        [DataMember(Name = "to")]
        public string Receiver { get; set; }
    }


    [DataContract]
    public class CollectedMessages
    {
        [DataMember(Name = "user")]
        public string Sender { get; set; }

        [DataMember(Name = "message")]
        public string Content { get; set; }

        [DataMember(Name = "time")]
        public DateTime Time { get; set; }
    }

    [DataContract]
    public class ClearWithUser
    {
        [DataMember(Name = "user")]
        public string User { get; set; }
    }
}
