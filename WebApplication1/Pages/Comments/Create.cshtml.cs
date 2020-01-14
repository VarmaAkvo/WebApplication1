using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;

namespace WebApplication1.Pages.Comments
{
    [Authorize]
    public class CreateModel : PageModel
    {
    }
}
