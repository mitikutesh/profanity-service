using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profanity.API.Helper;
using Profanity.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfanityController : ControllerBase
    {
        [HttpPost("check-text-for-profanity")]
        public async Task<ActionResult<ResponseModel>> Post([FromForm(Name = "file")] IFormFile file)
        {
            try
            {
                string text = await file.ToTextAsync(Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(text))
                {
                    return default;
                }
                var response = new ResponseModel();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpPost("add-words-to-profanity-list")]
        public async Task<IActionResult> PostAddToProfanityList(string word)
        {
            return default;
        }

        [HttpPost("remove-word-from-prfanity-list")]
        public async Task<IActionResult> PostRemoveFromProfanityList(string word)
        {
            return default;
        }

       }
}
