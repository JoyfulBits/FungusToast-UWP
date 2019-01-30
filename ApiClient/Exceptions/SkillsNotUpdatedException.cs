using System;
using ApiClient.Models;

namespace ApiClient.Exceptions
{
    public class SkillsNotUpdatedException : Exception
    {
        public SkillsNotUpdatedException(SkillExpenditureRequest skillExpenditureRequest) 
            : base($"Unable to updates skills for player with id '{skillExpenditureRequest.PlayerId}' for game with id {skillExpenditureRequest.GameId}")
        {
        }
    }
}