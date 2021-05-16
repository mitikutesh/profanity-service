using Profanity.Data.DTO;
using Profanity.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.Service.Interfaces
{
    public interface IProfanityService
    {
        (bool, List<string>, int, long)? ContainsProfanity(string term);

        Task<bool> AddProfanityAsync(ProfanityDTO profanity);

        Task<bool> RemoveProfanityAsync(ProfanityDTO profanity);

        Task<bool> ClearAsync();

        Task<bool> ClearAsync(Language language);

        Task<List<string>> GetAllProfanitiesAsync(Language language);
    }
}