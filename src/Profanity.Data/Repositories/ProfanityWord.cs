using Profanity.Data.DTO;
using Profanity.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Profanity.Data.Repositories
{
    public class ProfanityWord : IProfanityWord
    {
        private readonly ProfanityServiceDbContext _context;
        public ProfanityWord(ProfanityServiceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteProfanityAsync(ProfanityDTO profanityDTO)
        {
            try
            {
                var newProfanity = profanityDTO.ProfanityWord;

                var existing = await _context.ProfanityEntities.Where(a => a.Language == profanityDTO.Language).FirstOrDefaultAsync(); 
                if (existing != null)
                {
                    var existingProfanity = Encoding.UTF8.GetString(existing.ProfanityWord)?.Split(',').ToList();
                    existingProfanity = existingProfanity.Except(newProfanity).ToList();
                    var finalString = String.Join(',', existingProfanity);

                    existing.ProfanityWord = Encoding.UTF8.GetBytes(finalString);
                    _context.ProfanityEntities.Update(existing);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddToProfanityAsync(ProfanityDTO profanityDTO)
        {
            try
            {
                var newProfanity = profanityDTO.ProfanityWord.Distinct();

                var existing = await _context.ProfanityEntities.FirstOrDefaultAsync(a => a.Language == profanityDTO.Language);
                if (existing != null)
                {
                    var existingProfanity = Encoding.UTF8.GetString(existing.ProfanityWord)?.Split(',').ToList();
                    existingProfanity.AddRange(newProfanity);
                    existing.ProfanityWord = Encoding.UTF8.GetBytes(string.Join(',', existingProfanity));
                    _context.ProfanityEntities.Update(existing);
                }
                else
                {
                    var entity = new ProfanityEntity
                    {
                        Id = Guid.NewGuid(),
                        Language = profanityDTO.Language,
                        ProfanityWord = Encoding.UTF8.GetBytes(string.Join(',', newProfanity))
                    };


                    _context.ProfanityEntities.Add(entity);
                }
                
                return await _context.SaveChangesAsync() > 0;
                
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<ProfanityDTO> GetProfanityAsync(Guid id)
         => EntityToDTO(await _context.ProfanityEntities.FindAsync(id));

        public async Task<ProfanityDTO> GetProfanityByLanguageAsync(Language language)
          => EntityToDTO(await _context.ProfanityEntities.FirstOrDefaultAsync(a => a.Language == language));
        

        private ProfanityDTO EntityToDTO(ProfanityEntity entity) 
        {
            if (entity == null) return null;
            string text = Encoding.UTF8.GetString(entity.ProfanityWord);
            return new ProfanityDTO
            {
                Language = (Language)entity.Language,
                ProfanityWord = text?.Split(',').ToList()
            };
        }

        public async Task<bool> DeleteAllProfanityAsync()
        {
             _context.ProfanityEntities.Clear();
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAllProfanityAsync(Language language)
        {
            var byLanguage = _context.ProfanityEntities.Where(a => a.Language == language);
            _context.ProfanityEntities.RemoveRange(byLanguage);
            return await _context.SaveChangesAsync() > 0;
        }


    }

  
}
