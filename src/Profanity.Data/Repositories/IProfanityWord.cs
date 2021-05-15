using Profanity.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profanity.Data.Repositories
{
    public interface IProfanityWord
    {
        Task<bool> AddAsync(ProfanityDTO prospect);

        Task<List<ProfanityDTO>> GetProfanityListAsync();

        Task<ProfanityDTO> GetPrfanityAsync(int id);

        Task<bool> EditProfanityAsync(ProfanityDTO prospect);

        Task<bool> DeleteProfanityAsync(int id);
    }
}
