using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace ApiClient.Models
{
    public class SkillExpenditureRequest
    {
        public string PlayerId { get; }
        public Dictionary<int, SkillUpgrade> Upgrades { get; } = new Dictionary<int, SkillUpgrade>();

        public SkillExpenditureRequest(string playerId)
        {
            PlayerId = playerId;
        }

        public void IncreaseHypermutation()
        {
            var key = (int)Skills.Hypermutation;

            AddSpentPoint(key);
        }

        public void IncreaseRegeneration()
        {
            var key = (int)Skills.Regeneration;

            AddSpentPoint(key);
        }

        public void IncreaseAntiApoptosis()
        {
            var key = (int)Skills.AntiApoptosis;

            AddSpentPoint(key);
        }

        public void IncreaseBudding()
        {
            var key = (int)Skills.Budding;

            AddSpentPoint(key);
        }

        public void IncreaseMycotoxicity()
        {
            var key = (int)Skills.Mycotoxicity;

            AddSpentPoint(key);
        }

        public void IncreaseHydrophilia()
        {
            var key = (int)Skills.Hydrophilia;

            AddSpentPoint(key);
        }

        private void AddSpentPoint(int skillId)
        {
            if (Upgrades.ContainsKey(skillId))
            {
                Upgrades[skillId].PointsSpent++;
            }
            else
            {
                Upgrades[skillId] = new SkillUpgrade
                {
                    PointsSpent = 1
                };
            }
        }


        public void AddMoistureDroplet(int gridCellIndex)
        {
            var skillId = (int)Skills.Hydrophilia;
            if (Upgrades.ContainsKey(skillId))
            {
                Upgrades[skillId].ActiveCellChanges.Add(gridCellIndex);
            }
            else
            {
                var skillUpgrade = new SkillUpgrade();
                skillUpgrade.ActiveCellChanges.Add(gridCellIndex);
                Upgrades.Add(skillId, skillUpgrade);
            }
        }

        public void IncreaseSpores()
        {
            var key = (int)Skills.Hydrophilia;

            AddSpentPoint(key);
        }

        public void UseEyeDropper()
        {
            var key = (int)Skills.EyeDropper;

            AddSpentPoint(key);
        }
    }

    /// <summary>
    /// Created for API serialization purposes only
    /// </summary>
    public class PlayerSkillActiveCellChanges
    {
        public string PlayerId { get; set; }
        public int SkillId { get; set; }
        public List<int> Indexes { get; set; }
    }
}
