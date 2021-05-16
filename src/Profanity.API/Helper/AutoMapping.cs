using AutoMapper;
using Profanity.API.Model;
using Profanity.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profanity.API.Helper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<RequestModel, ProfanityDTO>(); // means you want to map from User to UserDTO
        }
    }
}
