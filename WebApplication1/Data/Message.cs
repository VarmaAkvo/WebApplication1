using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Areas.Identity.Data;

namespace WebApplication1.Data
{
    public class Message
    {
        public int ID { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public int PostID { get; set; }
        public string Action { get; set; }
        public bool Readed { get; set; }
        public DateTime UpdatedAtUTC { get; set; }

        public WebApplication1User Sender { get; set; }
        public WebApplication1User Receiver { get; set; }
        public Post Post { get; set; }
    }
}
