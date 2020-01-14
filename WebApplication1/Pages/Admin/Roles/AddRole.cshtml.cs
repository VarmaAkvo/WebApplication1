using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Areas.Identity.Data;

namespace WebApplication1.Pages.Admin.Roles
{
    public class AddRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<WebApplication1User> _userManager;

        public AddRoleModel(RoleManager<IdentityRole> roleManager,
            UserManager<WebApplication1User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
       
        public List<WebApplication1User> Users { get; set; }
        public SelectList Roles { get; set; }
        public Dictionary<string, string> UserRoles { get; set; }
        [BindProperty]
        public string Role { get; set; }
        [BindProperty]
        public string UserID { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Roles = new SelectList(_roleManager.Roles.Select(r => r.Name).ToList());
            Users = _userManager.Users.ToList();
            UserRoles = new Dictionary<string, string>();
            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles == null || roles.Count == 0)
                {
                    UserRoles[user.Id] = "None";
                }
                else
                {
                    UserRoles[user.Id] = String.Join(',', roles);
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(UserID);
            if (string.IsNullOrEmpty(Role))
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                return RedirectToAction("Get");
            }
            if (await _userManager.IsInRoleAsync(user, Role))
            {
               await _userManager.RemoveFromRoleAsync(user, Role);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, Role);
            }
            return RedirectToAction("Get");
        }
    }
}