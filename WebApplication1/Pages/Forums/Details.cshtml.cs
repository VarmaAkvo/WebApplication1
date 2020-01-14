using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Utilities;

namespace WebApplication1.Pages.Forums
{
    public class DetailsModel : PageModel
    {
        private readonly WebApplication1Context _ctx;

        public DetailsModel(WebApplication1Context ctx)
        {
            _ctx = ctx;
        }
        public class PostVM
        {
            public Post Post { get; set; }
            public int CommentCount { get; set; }
            public Comment NewestComment { get; set; }
        }

        public Forum Forum { get; set; }
        public PaginatedList<Post> Posts { get; set; }
        public List<PostVM> PostVMs { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, int pageIndex = 1)
        {
            Forum = await _ctx.Forums.FirstOrDefaultAsync(f => f.ID == id);
            if (Forum == null)
            {
                return NotFound();
            }
            var posts = _ctx.Posts.Include(p => p.User)
                .Where(p => p.ForumID == id).OrderByDescending(p => p.UpdatedAtUTC);
            Posts = await PaginatedList<Post>.CreateAsync(posts, pageIndex, 10);

            PostVMs = new List<PostVM>();
            var keys = Posts.Select(p => p.ID);
            var commentCounts = _ctx.Comments
                .Where(c => keys.Contains(c.PostID))
                .GroupBy(c => c.PostID)
                .Select(g => new { g.Key, Count = g.Count() });
            foreach (var post in Posts)
            {
                int commentCount = 0;
                if (commentCounts.Any(a => a.Key == post.ID))
                {
                    commentCount = commentCounts.First(a => a.Key == post.ID).Count;
                }
                var newestComment = _ctx.Comments.Include(c => c.User)
                    .Where(c => c.PostID == post.ID)
                    .OrderByDescending(c => c.CreatedAtUTC)
                    .FirstOrDefault();
                PostVMs.Add(new PostVM()
                {
                    Post = post,
                    CommentCount = commentCount,
                    NewestComment = newestComment
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateReadCountAsync(int postId)
        {
            var post = await _ctx.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }
            post.ReadCount += 1;
            await _ctx.SaveChangesAsync();
            return RedirectToPage("/Posts/Details", new { id = post.ID });
        }
    }
}