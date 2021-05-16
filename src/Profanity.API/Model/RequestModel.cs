using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;
using Profanity.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Profanity.API.Model
{
    public class RequestModel
    {
        public Language Language { get; set; }

        public List<string> ProfanityWord { get; set; }
    }

    public class RequestModelValidator : AbstractValidator<RequestModel>
    {
        public RequestModelValidator()
        {
            //RuleFor(req => req.Language).IsInEnum<RequestModel, Language>();
            RuleFor(req => req.Language).NotNull().WithMessage("Set the langague");
            RuleFor(a => a.ProfanityWord).NotNull().WithMessage("There should be atlease one word");
        }
    }
}
