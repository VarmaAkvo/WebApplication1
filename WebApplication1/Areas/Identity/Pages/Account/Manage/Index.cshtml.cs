﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Utilities;

namespace WebApplication1.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<WebApplication1User> _userManager;
        private readonly SignInManager<WebApplication1User> _signInManager;
        private readonly FileUpload _fileUpload;

        public IndexModel(
            UserManager<WebApplication1User> userManager,
            SignInManager<WebApplication1User> signInManager,
            FileUpload fileUpload)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileUpload = fileUpload;
        }

        public string Username { get; set; }

        public string Gravatar { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public IFormFile Gravatar { get; set; }
        }

        private async Task LoadAsync(WebApplication1User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            Gravatar = _fileUpload.GetImageUrl(user.Gravatar, "GravatarStorePath");

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Name = user.Name
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }
            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }
            if (Input.Gravatar != null)
            {
                var result = await _fileUpload.UploadGravatarAsync(Input.Gravatar);
                if (result.Successed)
                {
                    if (!string.IsNullOrEmpty(user.Gravatar))
                    {
                        _fileUpload.DeleteFile(user.Gravatar);
                    }
                    user.Gravatar = result.Path;
                }
                else
                {
                    ModelState.AddModelError("Input.Gravatar", result.ErrorMessage);
                    await LoadAsync(user);
                    return Page();
                }
            }
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
