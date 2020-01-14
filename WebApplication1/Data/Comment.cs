using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Areas.Identity.Data;

namespace WebApplication1.Data
{
    public class Comment
    {
        public int ID { get; set; }
        [Required]
        public string Content { get; set; }
        public string AttachmentPaths { get; set; }
        public DateTime CreatedAtUTC { get; set; }

        public int PostID { get; set; }
        public Post Post { get; set; }

        public string UserID { get; set; }
        public WebApplication1User User { get; set; }

        [ForeignKey("ReplyTo")]
        public int? ReplyToID { get; set; }
        public Comment ReplyTo { get; set; }
    }
}
