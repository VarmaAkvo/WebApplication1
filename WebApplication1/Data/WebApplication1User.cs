using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Data;

namespace WebApplication1.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the WebApplication1User class
    public class WebApplication1User : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string Gravatar { get; set; }
    }
}
