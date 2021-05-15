using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profanity.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return Ok();
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
