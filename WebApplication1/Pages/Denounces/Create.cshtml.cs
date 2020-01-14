using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;

namespace WebApplication1.Pages.Denounces
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly WebApplication1Context _ctx;
        private readonly UserManager<WebApplication1User> _userManger;
        public CreateModel(WebApplication1Context ctx, 
            UserManager<WebApplication1User> userManger)
        {
            _ctx = ctx;
            _userManger = userManger;
        }

        public WebApplication1User Denounced { get; set; }
        public string DContent { get; set; }
        [BindProperty]
        public Denounce Denounce { get; set; }

        public async Task<IActionResult> OnGetAsync(bool isPost, int id)
        {
            if (isPost)
            {
                var post = await _ctx.Posts.Include(p => p.User).FirstAsync(p => p.ID == id);
                if (post == null)
                {
                    return BadRequest();
                }
                DContent = post.Content;
                Denounced = post.User;
            }
            else
            {
                var comment = await _ctx.Comments.Include(p => p.User).FirstAsync(c => c.ID == id);
                if (comment == null)
                {
                    return BadRequest();
                }
                DContent = comment.Content;
                Denounced = comment.User;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool isPost, int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var denouncingID = _userManger.GetUserId(User);
            Denounce.DenouncingID = denouncingID;
            Denounce.IsPost = isPost;
            Denounce.PostOrCommentID = id;
            Denounce.CreatedAtUTC = DateTime.UtcNow;

            _ctx.Denounces.Add(Denounce);
            await _ctx.SaveChangesAsync();

            int postId;
            if (isPost)
            {
                postId = _ctx.Posts.Find(id).ID;
            }
            else
            {
                postId = _ctx.Comments.Find(id).PostID;
            }
            return RedirectToPage("/Posts/Details", new { id = postId });
        }
    }
}
