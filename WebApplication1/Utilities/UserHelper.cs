using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Areas.Identity.Data;

namespace WebApplication1.Utilities
{
    public class UserHelper
    {
        private readonly UserManager<WebApplication1User> _userManager;
        private readonly IConfiguration _config;
        public UserHelper(UserManager<WebApplication1User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<string> GetUserName(ClaimsPrincipal user)
        {
            var u = await _userManager.GetUserAsync(user);
            return u.Name;
        }

        public string GetGravatarUrl(string gravatarPath)
        {
            string fileName = Path.GetFileName(gravatarPath);
            return $"/{_config.GetValue<string>("GravatarStorePath")}/{fileName}";
        }
    }
}
