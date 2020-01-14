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
    public class EditModel : PageModel
    {
        private readonly WebApplication1Context _ctx;
        private readonly FileUpload _fileUpload;

        public EditModel(WebApplication1Context ctx, FileUpload fileUpload)
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

        public string Icon { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var forum =  await _ctx.Forums.FindAsync(id);
            if (forum == null)
            {
                return NotFound();
            }
            Input = new InputModel()
            {
                Name = forum.Name,
                Description = forum.Description
            };
            Icon = _fileUpload.GetImageUrl(forum.Icon, "ForumIconStorePath");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var forum = await _ctx.Forums.FindAsync(id);
            if (forum == null)
            {
                return NotFound();
            }
            if (Input.Icon != null)
            {
                var result = await _fileUpload.UploadAsync(Input.Icon, "ForumIconStorePath");
                if (result.Successed)
                {
                    if (!string.IsNullOrEmpty(forum.Icon))
                    {
                        _fileUpload.DeleteFile(forum.Icon);
                    }
                    forum.Icon = result.Path;
                }
                else
                {
                    ModelState.AddModelError("Input.Icon", result.ErrorMessage);
                    return Page();
                }
            }

            forum.Name = Input.Name;
            forum.Description = Input.Description;

            await _ctx.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}