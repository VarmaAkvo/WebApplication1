using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1
{
    public class IndexModel : PageModel
    {
        private readonly WebApplication1Context _ctx;

        public IndexModel(WebApplication1Context ctx)
        {
            _ctx = ctx;
        }

        public List<Forum> Forums { get; set; }

        public IActionResult OnGet()
        {
            Forums = _ctx.Forums.AsNoTracking().ToList();
            return Page();
        }
    }
}