using AutoMapper;
using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
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
        public async Task<ApiResponse> Post([FromForm(Name = "file")] IFormFile file, Language language)
        {
            if (!ModelState.IsValid)
            {
                throw new ApiProblemDetailsException(ModelState);
            }

            try
            {
                var l = language;
                var text = await file.ToByteArray(Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(text)) return default;
                
                var response = new ResponseModel();
                response.IsProfanity = _profanityService.ContainsProfanity(text);
                response.FoundProfanityWords = _profanityService.FindAllProfanities(text);
                response.Count = response.FoundProfanityWords.Count();
                response.ElapasedTime = 0;

                return new ApiResponse(response);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        //throw new ApiProblemDetailsException($"Record with id: {id} does not exist.", Status404NotFound); 
        [HttpPut("add-words-to-profanity-list")]
        public async Task<IActionResult> PostAddToProfanityList([FromBody]RequestModel request)
        {
            var result  = _mapper.Map<RequestModel, ProfanityDTO>(request);
            var response = await _profanityService.AddProfanityAsync(result);
            return Ok(response);
        }

        [HttpPost("remove-word-from-prfanity-list")]
        public async Task<ActionResult<bool>> PostRemoveFromProfanityList([FromBody]RequestModel request)
        {
            return Ok(await _profanityService.RemoveProfanityAsync(_mapper.Map<RequestModel, ProfanityDTO>(request)));
        }

        [HttpGet("get-all-profanity")]
        public async Task<ActionResult<List<string>>> Get(Language language)
        {
            return Ok(await _profanityService.GetAllProfanitiesAsync(language));
        }

        [HttpDelete("clear-profanity")]
        public async Task<ActionResult<bool>> DeleteByLanguage(Language language)
        {
            return Ok(await _profanityService.ClearAsync(language));
        }

        [HttpDelete("clear-all-profanity")]
        public async Task<ActionResult<bool>> Delete()
        {
            return Ok(await _profanityService.ClearAsync());
        }
    }

    
}
