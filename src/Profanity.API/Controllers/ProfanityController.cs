using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profanity.API.Helper;
using Profanity.API.Model;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Service.Interfaces;
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
        private readonly IProfanityService _profanityService;
        private readonly IMapper _mapper;

        public ProfanityController(IProfanityService profanityService, IMapper mapper)
        {
            _profanityService = profanityService;
            _mapper = mapper;
        }
        [HttpPost("check-text-for-profanity")]
        public async Task<ActionResult<ResponseModel>> Post([FromForm(Name = "file")] IFormFile file)
        {
            try
            {
                var text = await file.ToByteArray(Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(text))
                {
                    return default;
                }
                
                var response = new ResponseModel();
                response.IsProfanity = _profanityService.IsProfanity(text);
                response.FoundProfanityWords = _profanityService.FindAllProfanities(text);
                response.Count = response.FoundProfanityWords.Count();
                response.ElapasedTime = 0;

                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpPost("add-words-to-profanity-list")]
        public async Task<IActionResult> PostAddToProfanityList(RequestModel request)
        {
            var result  = _mapper.Map<RequestModel, ProfanityDTO>(request);
            _profanityService.AddProfanity(request.ProfanityWord);
            return default;
        }

        [HttpPost("remove-word-from-prfanity-list")]
        public async Task<IActionResult> PostRemoveFromProfanityList(RequestModel request)
        {
            return default;
        }

       }
}
