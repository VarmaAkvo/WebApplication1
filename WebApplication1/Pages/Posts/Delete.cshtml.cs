using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Utilities;

namespace WebApplication1.Pages.Posts
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly WebApplication1Context _ctx;
        private readonly UserManager<WebApplication1User> _userManager;
        private readonly FileUpload _fileUpload;

        public DeleteModel(WebApplication1Context ctx, 
            UserManager<WebApplication1User> userManager,
            FileUpload fileUpload)
        {
            _ctx = ctx;
            _userManager = userManager;
            _fileUpload = fileUpload;
        }

        public Post Post { get; set; }

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
            Post = post;
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
            if (!string.IsNullOrEmpty(post.AttachmentPaths))
            {
                var paths = post.AttachmentPaths.Split(';');
                _fileUpload.DeleteFiles(paths);
            }
            var comments = _ctx.Comments.Where(c => c.PostID == post.ID);
            if (comments.Any())
            {
                foreach (var comment in comments)
                {
                    if (!string.IsNullOrEmpty(comment.AttachmentPaths))
                    {
                        var paths = comment.AttachmentPaths.Split(';');
                        _fileUpload.DeleteFiles(paths);
                    }
                }
                _ctx.Comments.RemoveRange(comments);
            }
            _ctx.Posts.Remove(post);
            await _ctx.SaveChangesAsync();
            return RedirectToPage("/Index");
        }
    }
}
