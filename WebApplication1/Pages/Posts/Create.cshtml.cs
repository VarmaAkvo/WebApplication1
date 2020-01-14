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
    public class CreateModel : PageModel
    {
        private readonly WebApplication1Context _ctx;
        private readonly UserManager<WebApplication1User> _userManager;

        public CreateModel(WebApplication1Context ctx, UserManager<WebApplication1User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }
        
        public class InputModel
        {
            public int ForumID { get; set; }
            [Required]
            [StringLength(20)]
            public string Title { get; set; }
            [Required]
            public string Content { get; set; }
            public string AttachmentPaths { get; set; }
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public IActionResult OnGet(int forumId)
        {
            if (_ctx.Forums.Any(f => f.ID == forumId))
            {
                Input = new InputModel();
                Input.ForumID = forumId;
                return Page();
            }
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!_ctx.Forums.Any(f => f.ID == Input.ForumID))
            {
                return BadRequest();
            }
            var userId = _userManager.GetUserId(User);
            var newPost = new Post()
            {
                Title = Input.Title,
                Content = Input.Content,
                ForumID = Input.ForumID,
                AttachmentPaths = Input.AttachmentPaths,
                CreatedAtUTC = DateTime.UtcNow,
                UpdatedAtUTC = DateTime.UtcNow,
                UserID = userId
            };
            _ctx.Posts.Add(newPost);
            await _ctx.SaveChangesAsync();
            return RedirectToPage("./Details", new { id = newPost.ID });
        }
    }
}