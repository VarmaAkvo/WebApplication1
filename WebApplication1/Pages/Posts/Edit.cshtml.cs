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

namespace WebApplication1.Pages.Posts
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly WebApplication1Context _ctx;
        private readonly UserManager<WebApplication1User> _userManager;

        public EditModel(WebApplication1Context ctx, 
            UserManager<WebApplication1User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public class InputModel
        {
            public int ID { get; set; }
            [Required]
            [StringLength(20)]
            public string Title { get; set; }
            [Required]
            public string Content { get; set; }
            public string AttachmentPaths { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var post = await _ctx.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            if (post.UserID != userId)
            {
                return Forbid();
            }
            Input = new InputModel()
            {
                ID = post.ID,
                Title = post.Title,
                Content = post.Content
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var post = await _ctx.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            if (post.UserID != userId)
            {
                return Forbid();
            }
            post.Title = Input.Title;
            post.Content = Input.Content;
            post.AttachmentPaths = Input.AttachmentPaths;
            await _ctx.SaveChangesAsync();
            return RedirectToPage("./Details", new { id });
        }
    }
}
