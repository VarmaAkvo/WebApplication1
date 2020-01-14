using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Areas.Identity.Data;

namespace WebApplication1.Data
{
    public class Post
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public int ReadCount { get; set; }
        public string AttachmentPaths { get; set; }
        public DateTime CreatedAtUTC { get; set; }
        public DateTime UpdatedAtUTC { get; set; }

        public string UserID { get; set; }
        public WebApplication1User User { get; set; }

        public int ForumID { get; set; }
        public Forum Forum { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
