using FluentValidation;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Profanity.Data.Entities
{
    public class ProfanityEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Key]
        
        public Language Language { get; set; }
        public byte[] ProfanityWord { get; set; }
    }

    public class ProfanityEntityValidator : AbstractValidator<ProfanityEntity>
    {
        public ProfanityEntityValidator()
        {
            //RuleFor(req => req.Language).IsInEnum<RequestModel, Language>();
            RuleFor(req => req.Language).NotNull().WithMessage("Set the langague");
            RuleFor(a => a.ProfanityWord).NotNull().WithMessage("There should be atlease one word");
        }
    }

    public enum Language
    {
        EN,
        FI,
        SWE
    }
  
}
