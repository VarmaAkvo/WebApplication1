using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using WebApplication1.Utilities;

namespace WebApplication1
{
    public class TrixUploadModel : PageModel
    {
        private readonly FileUpload _fileUpload;

        public TrixUploadModel(FileUpload fileUpload)
        {
            _fileUpload = fileUpload;
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            var result = await _fileUpload.UploadTrixAttachmentAsync(file);
            if (result.Successed)
            {
                return new JsonResult(new
                {
                    successed = true,
                    url = _fileUpload.GetTrixAttachmentUrl(result.Path),
                    path = result.Path
                });
            }
            else
            {
                return new JsonResult(new
                {
                    successed = false,
                    errorMessage = result.ErrorMessage
                });
            }
        }

        public  void OnPostRemoveAttachment(string url)
        {
            _fileUpload.DeleteFileByUrl(url);
        }
    }
}