using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profanity.API.Helper;
using Profanity.API.Model;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.API.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        [HttpPost(EndPoints.CheckProfanity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ApiResponse> Post(IFormFile file, Language language)
        {
            if (!ModelState.IsValid)
            {
                throw new ApiProblemDetailsException(ModelState);
            }
            
            try
            {
                if (file == null) throw new ApiException("No file uploaded.");
                var l = language;
                var text = await file.ToByteArray(Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(text)) return default;

                var result = _profanityService.ContainsProfanity(text);

                var response = new ResponseModel
                {
                    IsProfanity = result.Value.Item1,
                    FoundProfanityWords = result.Value.Item2,
                    Count = result.Value.Item3,
                    ElapasedTime = result.Value.Item4
                };
                return new ApiResponse(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut(EndPoints.AddWordToProfanity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostAddToProfanityList([FromBody] RequestModel request)
        {
            if (!ModelState.IsValid)
            {
                throw new ApiProblemDetailsException(ModelState);
            }
            var result = _mapper.Map<RequestModel, ProfanityDTO>(request);
            var response = await _profanityService.AddProfanityAsync(result);
            return Ok(response);
        }

        [HttpPost(EndPoints.RemoveWordFromProfanity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<bool>> PostRemoveFromProfanityList([FromBody] RequestModel request)
        {
            if (!ModelState.IsValid)
            {
                throw new ApiProblemDetailsException(ModelState);
            }

            return Ok(await _profanityService.RemoveProfanityAsync(_mapper.Map<RequestModel, ProfanityDTO>(request)));
        }

        [HttpGet(EndPoints.GetProfanitites)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<List<string>>> Get(Language language)
        {
            if (!ModelState.IsValid)
            {
                throw new ApiProblemDetailsException(ModelState);
            }
            return Ok(await _profanityService.GetAllProfanitiesAsync(language));
        }

        [HttpDelete(EndPoints.ClearSpecificProfanitites)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<bool>> DeleteByLanguage(Language language)
        {
            if (!ModelState.IsValid)
            {
                throw new ApiProblemDetailsException(ModelState);
            }
            return Ok(await _profanityService.ClearAsync(language));
        }

        [HttpDelete(EndPoints.ClearAllProfanities)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<bool>> Delete()
        {
            return Ok(await _profanityService.ClearAsync());
        }
    }
}