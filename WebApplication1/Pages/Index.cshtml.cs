using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication1.Data;
using WebApplication1.Utilities;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly WebApplication1Context _ctx;
        private readonly FileUpload _fileUpload;

        public IndexModel(ILogger<IndexModel> logger, WebApplication1Context ctx, FileUpload fileUpload)
        {
            _logger = logger;
            _ctx = ctx;
            _fileUpload = fileUpload;
        }

        public class ForumVM
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
            public int PostCount { get; set; }
            public Post NewestPost { get; set; }
        }
        public List<ForumVM> Forums { get; set; }
        public List<Post> Posts { get; set; }

        public void OnGet()
        {
            var forums = _ctx.Forums.AsNoTracking().ToList();
            Forums = new List<ForumVM>();
            var postCounts = _ctx.Posts.GroupBy(p => p.ForumID)
                .Select(g => new { g.Key, Count = g.Count()});
            foreach (var forum in forums)
            {
                var temp = postCounts.FirstOrDefault(a => a.Key == forum.ID);
                int postCount = temp == null ? 0 : temp.Count;
                Post newestPost = null;
                if (postCount != 0)
                {
                    newestPost = _ctx.Posts.Include(p => p.User)
                        .Where(p => p.ForumID == forum.ID)
                        .OrderByDescending(p => p.UpdatedAtUTC)
                        .FirstOrDefault();
                }
                Forums.Add(new ForumVM()
                {
                    ID = forum.ID,
                    Name = forum.Name,
                    Description = forum.Description,
                    Icon = _fileUpload.GetForumIconUrl(forum.Icon),
                    PostCount = postCount,
                    NewestPost = newestPost
                });
            }
            //get newest posts
            Posts = _ctx.Posts.Include(p => p.User).AsNoTracking()
                .OrderByDescending(p => p.UpdatedAtUTC).Take(5).ToList();
        }
    }
}
