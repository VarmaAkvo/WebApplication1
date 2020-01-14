using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Utilities;

namespace WebApplication1.Pages.Admin.Forums
{
    public class DeleteModel : PageModel
    {
        private readonly WebApplication1Context _ctx;
        private readonly FileUpload _fileUpload;

        public DeleteModel(WebApplication1Context ctx, FileUpload fileUpload)
        {
            _ctx = ctx;
            _fileUpload = fileUpload;
        }

        public Forum Forum { get; set; }
        public string Icon { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Forum = await _ctx.Forums.FindAsync(id);
            if (Forum == null)
            {
                return NotFound();
            }
            Icon = _fileUpload.GetForumIconUrl(Forum.Icon);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var forum = await _ctx.Forums.FindAsync(id);
            if (forum == null)
            {
                return NotFound();
            }
            _fileUpload.DeleteFile(forum.Icon);
            _ctx.Forums.Remove(forum);
            await _ctx.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}