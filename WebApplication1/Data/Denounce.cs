using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Areas.Identity.Data;

namespace WebApplication1.Data
{
    public class Denounce
    {
        public int ID { get; set; }
        public string DenouncedID { get; set; }
        public bool IsPost { get; set; }
        public int PostOrCommentID { get; set; }
        public string DenouncingID { get; set; }
        [Required]
        public string Reason { get; set; }
        public bool HasHandled { get; set; }
        public DateTime CreatedAtUTC { get; set; }

        public WebApplication1User Denounced { get; set; }
        public WebApplication1User Denouncing { get; set; }
    }
}
