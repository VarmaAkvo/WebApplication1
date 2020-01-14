using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Hubs;
using WebApplication1.Utilities;

namespace WebApplication1.Pages.Posts
{
    public class DetailsModel : PageModel
    {
        private readonly WebApplication1Context _ctx;
        private readonly UserManager<WebApplication1User> _userManager;
        private readonly IHubContext<MessageHub> _hubContext;

        public DetailsModel(WebApplication1Context ctx, 
            UserManager<WebApplication1User> userManager,
            IHubContext<MessageHub> hubContext)
        {
            _ctx = ctx;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public Post Post { get; set; }
        public PaginatedList<Comment> Comments { get; set; }
        public Dictionary<int, PaginatedList<Comment>> Replies { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, int pageIndex = 1)
        {
            Post = await _ctx.Posts
                .Include(p => p.User)
                .AsNoTracking()
                .FirstAsync(p => p.ID == id);
            if (Post == null)
            {
                return NotFound();
            }
            var comments = _ctx.Comments
                .Include(c => c.User)
                .Where(c => c.PostID == Post.ID && c.ReplyToID == null)
                .OrderByDescending(c => c.CreatedAtUTC)
                .AsNoTracking();
            Comments = await PaginatedList<Comment>.CreateAsync(comments, pageIndex, 10);
            Replies = new Dictionary<int, PaginatedList<Comment>>();
            if (Comments.Any())
            {
                foreach (var comment in Comments)
                {
                    var replies = _ctx.Comments.Include(c => c.User)
                        .Include(c => c.ReplyTo)
                            .ThenInclude(rt => rt.User)
                        .Where(c => c.ReplyToID == comment.ID)
                        .OrderByDescending(c => c.CreatedAtUTC)
                        .AsNoTracking();
                    var pReplies = await PaginatedList<Comment>.CreateAsync(replies, 1, 5);
                    Replies.Add(comment.ID, pReplies);
                }
            }
            return Page();
        }

        public class InputModel
        {
            public int PostID { get; set; }
            public int? ReplyToID { get; set; }
            [Required]
            public string Content { get; set; }
            public string AttachmentPaths { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnPostCommentAsync()
        {
            var post = await _ctx.Posts.FindAsync(Input.PostID);
            if (post == null)
            {
                return NotFound();
            }
            var user =  await _userManager.GetUserAsync(User);
            var mentions = await ProcessMentionAsync(Input, user);
            var newComment = new Comment()
            {
                Content = Input.Content,
                AttachmentPaths = Input.AttachmentPaths,
                CreatedAtUTC = DateTime.UtcNow,
                ReplyToID = Input.ReplyToID,
                PostID = Input.PostID,
                UserID = user.Id
            };
            _ctx.Comments.Add(newComment);
            post.UpdatedAtUTC = DateTime.UtcNow;
            await _ctx.SaveChangesAsync();
            //Comment message
            var receiver = await _userManager.FindByIdAsync(post.UserID);
            if (receiver.Id != user.Id)
            {
                var newMessage = new Message()
                {
                    ReceiverID = receiver.Id,
                    SenderID = user.Id,
                    Action = "comment",
                    PostID = post.ID,
                    Readed = false,
                    UpdatedAtUTC = DateTime.UtcNow
                };
                _ctx.Messages.Add(newMessage);
                await _ctx.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync(receiver.Email);
            }
            //Mention message
            if (mentions.Any())
            {
                foreach (var muser in mentions)
                {
                    _ctx.Messages.Add(new Message()
                    {
                        ReceiverID = muser.Id,
                        SenderID = user.Id,
                        Action = "mention",
                        PostID = post.ID,
                        Readed = false,
                        UpdatedAtUTC = DateTime.UtcNow
                    });
                }
                await _ctx.SaveChangesAsync();
                foreach (var muser in mentions)
                {
                    await _hubContext.Clients.All.SendAsync(muser.Email);
                }
            }
            return RedirectToAction("Get", new { id = Input.PostID });
        }
        //返回被@的实际存在的用户列表（不含重复用户，不含当前用户）
        private async Task<List<WebApplication1User>> ProcessMentionAsync(InputModel input,
            WebApplication1User user)
        {
            var mentions = new List<WebApplication1User>();
            var names = new Dictionary<string, string>();
            string content = input.Content;
            string pattern = @"\s(@[^\s]+)\s";
            Match match = Regex.Match(content, pattern);
            while (!string.IsNullOrEmpty(match.Value))
            {
                string name = match.Groups[1].ToString().Substring(1);
                if (_userManager.Users.Any(u => u.Name == name))
                {
                    content = content.Replace(match.Value, $@" <span class=""text-primary"">@{name}</span> ");
                    match = Regex.Match(content, pattern);
                    if (!names.ContainsKey(name))
                    {
                        names.Add(name, name);
                        var receiver = await _userManager.Users.FirstAsync(u => u.Name == name);
                        mentions.Add(receiver);
                    }
                }
            }
            input.Content = content;
            mentions.Remove(user);
            return mentions;
        }

        public async Task<PartialViewResult> OnGetRepliesAsync(int commentId, int pageIndex)
        {
            var replies = _ctx.Comments.Include(c => c.User)
                        .Include(c => c.ReplyTo)
                            .ThenInclude(rt => rt.User)
                        .Where(c => c.ReplyToID == commentId)
                        .OrderByDescending(c => c.CreatedAtUTC)
                        .AsNoTracking();
            var pReplies = await PaginatedList<Comment>.CreateAsync(replies, pageIndex, 5);
            return Partial("_ReplyAreaPartial", pReplies);
        }
    }
}
