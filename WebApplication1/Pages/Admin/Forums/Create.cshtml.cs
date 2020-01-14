using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Utilities;

namespace WebApplication1.Pages.Admin.Forums
{
    public class CreateModel : PageModel
    {
        private readonly WebApplication1Context _ctx;
        private readonly FileUpload _fileUpload;
        public CreateModel(WebApplication1Context ctx,FileUpload fileUpload)
        {
            _ctx = ctx;
            _fileUpload = fileUpload;
        }

        public class InputModel
        {
            [Required]
            [StringLength(15)]
            public string Name { get; set; }
            [Required]
            [StringLength(50)]
            public string Description { get; set; }
            public IFormFile Icon { get; set; }
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var newForum = new Forum()
                {
                    Name = Input.Name,
                    Description = Input.Description
                };
                if (Input.Icon != null)
                {
                    var result = await _fileUpload.UploadAsync(Input.Icon, "ForumIconStorePath");
                    if (result.Successed)
                    {
                        newForum.Icon = result.Path;
                    }
                    else
                    {
                        ModelState.AddModelError("Input.Icon", result.ErrorMessage);
                        return Page();
                    }
                }
                _ctx.Forums.Add(newForum);
                await _ctx.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
