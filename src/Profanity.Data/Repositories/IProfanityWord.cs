using Profanity.Data.DTO;
using Profanity.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profanity.Data.Repositories
{
    public interface IProfanityWord
    {
        Task<ProfanityDTO> GetProfanityAsync(Guid id);

        Task<ProfanityDTO> GetProfanityByLanguageAsync(Language language);

        Task<bool> AddToProfanityAsync(ProfanityDTO prospect);

        Task<bool> DeleteProfanityAsync(ProfanityDTO profanityDTO);

        Task<bool> DeleteAllProfanityAsync();
    }
}
