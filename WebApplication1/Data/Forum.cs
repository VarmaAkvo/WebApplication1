using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public class Forum
    {
        public int ID { get; set; }
        [Required]
        [StringLength(15)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
        public string Icon { get; set; }

        public List<Post> Posts { get; set; }
    }
}
