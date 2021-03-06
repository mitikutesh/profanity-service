using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.API.Helper
{
    public static class Extensions
    {
        public static async Task<string> ToByteArray(this IFormFile formFile, Encoding encoding)
        {
            if (formFile == null) return null;
            if (formFile.Length > 0)
            {
                using (var fileStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(fileStream);
                    string text = encoding.GetString(fileStream.ToArray());
                    fileStream.Flush();

                    return text;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
