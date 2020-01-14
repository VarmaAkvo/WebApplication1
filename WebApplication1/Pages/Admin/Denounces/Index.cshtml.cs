using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Utilities;

namespace WebApplication1.Pages.Admin.Denounces
{
    public class IndexModel : PageModel
    {
        private readonly WebApplication1Context _ctx;

        public IndexModel(WebApplication1Context ctx)
        {
            _ctx = ctx;
        }

        public PaginatedList<Denounce> Denounces { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            var denounces = _ctx.Denounces.AsNoTracking()
                .Include(d => d.Denounced)
                .Include(d => d.Denouncing)
                .Where(d => d.HasHandled == false)
                .OrderBy(d => d.CreatedAtUTC);
            Denounces = await PaginatedList<Denounce>.CreateAsync(denounces, pageIndex, 10);
            return Page();
        }
    }
}
