using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Utilities;

namespace WebApplication1.Pages.Messages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly WebApplication1Context _ctx;
        private readonly UserManager<WebApplication1User> _userManager;
        private readonly string[] ValidActions = { "comment", "mention" };

        public IndexModel(WebApplication1Context ctx,
            UserManager<WebApplication1User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public PaginatedList<Message> Messages { get; set; }
        public string SelectedAction { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1, string action = "")
        {
            var user = await _userManager.GetUserAsync(User);
            var messages = _ctx.Messages
                .Include(m => m.Post)
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .Where(m => m.ReceiverID == user.Id)
                .OrderBy(m => m.Readed)
                .ThenByDescending(m => m.UpdatedAtUTC)
                .AsNoTracking();
            if (!string.IsNullOrEmpty(action))
            {
                if (ValidActions.Contains(action))
                {
                    messages = messages.Where(m => m.Action == action);
                }
                else
                {
                    return BadRequest();
                }
            }
            SelectedAction = action;
            Messages = await PaginatedList<Message>.CreateAsync(messages, pageIndex, 10);
            return Page();
        }

        public async Task<IActionResult> OnPostCheckAsync(int msgId)
        {
            var msg = await _ctx.Messages.FindAsync(msgId);
            msg.Readed = true;
            msg.UpdatedAtUTC = DateTime.UtcNow;
            await _ctx.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostCheckAllAsync(string action)
        {
            var user = await _userManager.GetUserAsync(User);
            var messages = _ctx.Messages
                .Where(m => m.ReceiverID == user.Id &&
                    m.Readed == false);
            if (!ValidActions.Contains(action))
            {
                return BadRequest();
            }
            if (!string.IsNullOrEmpty(action))
            {
                messages = messages.Where(m => m.Action == action);
            }
            foreach (var msg in messages)
            {
                msg.Readed = true;
                msg.UpdatedAtUTC = DateTime.UtcNow;
            }
            await _ctx.SaveChangesAsync();
            return RedirectToPage("./Index", new { action });
        }
    }
}
