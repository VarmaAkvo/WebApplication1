using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.Admin.Roles
{
    public class CreateModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [Required]
        [BindProperty]
        public string RoleName { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(RoleName))
            {
                return BadRequest();
            }
            if (await _roleManager.RoleExistsAsync(RoleName))
            {
                ModelState.AddModelError("RoleName", "The Role has already existed.");
                return Page();
            }
            else
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleName));
                return RedirectToPage("/index");
            }
        }
    }
}