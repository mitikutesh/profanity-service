using System;
using System.ComponentModel.DataAnnotations;

namespace Profanity.Data.Entities
{
    public class ProfanityEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string ProfanityWord { get; set; }
    }
}
