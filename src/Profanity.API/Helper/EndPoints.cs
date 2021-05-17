using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profanity.API.Helper
{
    public struct EndPoints
    {
        public const string CheckProfanity = "check-text-for-profanity";
        public const string AddWordToProfanity = "add-text-to-profanity";
        public const string RemoveWordFromProfanity = "remove-word-from-profanity-list";
        public const string GetProfanitites = "get-profanity";
        public const string ClearAllProfanities = "clear-all-profanity";
        public const string ClearSpecificProfanitites = "clear-profanity";

        public const string HealthQuickCheck = "health";
        public const string HealthService = "health/services";
        public const string HealthDatabase = "health/database";
    }
}
