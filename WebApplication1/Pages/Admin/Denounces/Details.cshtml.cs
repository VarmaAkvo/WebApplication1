using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages.Admin.Denounces
{
    public class DetailsModel : PageModel
    {
        private readonly WebApplication1Context _ctx;

        public DetailsModel(WebApplication1Context ctx)
        {
            _ctx = ctx;
        }

        public Denounce Denounce { get; set; }
        public string DContent { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Denounce = await _ctx.Denounces.Include(d => d.Denounced)
                .FirstAsync(d => d.ID == id);
            if (Denounce == null || Denounce.HasHandled)
            {
                return NotFound();
            }
            if (Denounce.IsPost)
            {
                DContent = _ctx.Posts.Find(Denounce.PostOrCommentID).Content;
            }
            else
            {
                DContent = _ctx.Comments.Find(Denounce.PostOrCommentID).Content;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, bool agree)
        {
            var denounce = await _ctx.Denounces.FindAsync(id);
            if (denounce == null || denounce.HasHandled)
            {
                return NotFound();
            }
            if (agree)
            {
                denounce.HasHandled = true;
            }
            else
            {
                _ctx.Denounces.Remove(denounce);
            }
            await _ctx.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
