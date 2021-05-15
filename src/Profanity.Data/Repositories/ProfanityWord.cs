using Profanity.Data.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profanity.Data.Repositories
{
    public class ProfanityWord : IProfanityWord
    {
        private readonly ProfanityServiceDbContext _context;
        public ProfanityWord(ProfanityServiceDbContext context)
        {
            _context = context;
        }
        public Task<bool> AddAsync(ProfanityDTO profanityDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProfanityAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditProfanityAsync(ProfanityDTO prospect)
        {
            throw new NotImplementedException();
        }

        public Task<ProfanityDTO> GetPrfanityAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProfanityDTO>> GetProfanityListAsync()
        {
            throw new NotImplementedException();
        }
    }
}
