using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
    public class FileUploadService
    {
        public static async Task<string> Upload(IFormFile file)
        {
            if(file.OpenReadStream().Length > 2 * 1024 * 1024)
            {
                throw new InvalidOperationException("File size exceeds the limit of 2 MB.");
            }

            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if(!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }
    }
}