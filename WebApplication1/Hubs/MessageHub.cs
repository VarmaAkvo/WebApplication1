using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;

namespace WebApplication1.Hubs
{
    public class MessageHub : Hub
    {
        private readonly WebApplication1Context _ctx;
        private readonly UserManager<WebApplication1User> _userManager;

        public MessageHub(WebApplication1Context ctx,
            UserManager<WebApplication1User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public async Task<int> GetUnreadedMessagesCount(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            int count = _ctx.Messages
                .Where(m => m.ReceiverID == user.Id && m.Readed == false)
                .Count();
            return count;
        }
    }
}
