using System.Collections.Generic;

namespace ApiClient.Models
{
    public class SkillExpenditureRequest
    {
        public string PlayerId { get; }
        public Dictionary<int, ActiveSkillChanges> ActiveSkillChanges { get; } = new Dictionary<int, ActiveSkillChanges>();
        public Dictionary<int, int> SkillUpgrades = new Dictionary<int, int>();

        public SkillExpenditureRequest(string playerId)
        {
            PlayerId = playerId;
        }

        public void IncreaseHypermutation()
        {
            var key = (int)PassiveSkills.Hypermutation;

            AddSpentPoint(key);
        }

        public void IncreaseRegeneration()
        {
            var key = (int)PassiveSkills.Regeneration;

            AddSpentPoint(key);
        }

        public void IncreaseAntiApoptosis()
        {
            var key = (int)PassiveSkills.AntiApoptosis;

            AddSpentPoint(key);
        }

        public void IncreaseBudding()
        {
            var key = (int)PassiveSkills.Budding;

            AddSpentPoint(key);
        }

        public void IncreaseMycotoxicity()
        {
            var key = (int)PassiveSkills.Mycotoxicity;

            AddSpentPoint(key);
        }

        public void IncreaseHydrophilia()
        {
            var key = (int)PassiveSkills.Hydrophilia;

            AddSpentPoint(key);
        }

        private void AddSpentPoint(int passiveSkillId)
        {
            if (SkillUpgrades.ContainsKey(passiveSkillId))
            {
                SkillUpgrades[passiveSkillId]++;
            }
            else
            {
                SkillUpgrades[passiveSkillId] = 1;
            }
        }

        private void AddSpentActionPoint(int activeSkillId)
        {
            if (ActiveSkillChanges.ContainsKey(activeSkillId))
            {
                ActiveSkillChanges[activeSkillId].PointsSpent++;
            }
            else
            {
                ActiveSkillChanges[activeSkillId] = new ActiveSkillChanges
                {
                    PointsSpent = 1
                };
            }
        }


        public void AddMoistureDroplet(int gridCellIndex)
        {
            var skillId = (int)ActiveSkills.EyeDropper;
            if (ActiveSkillChanges.ContainsKey(skillId))
            {
                ActiveSkillChanges[skillId].ActiveCellChanges.Add(gridCellIndex);
            }
            else
            {
                var skillUpgrade = new ActiveSkillChanges();
                skillUpgrade.ActiveCellChanges.Add(gridCellIndex);
                ActiveSkillChanges.Add(skillId, skillUpgrade);
            }
        }

        public void IncreaseSpores()
        {
            var key = (int)PassiveSkills.Spores;

            AddSpentPoint(key);
        }

        public void UseEyeDropper()
        {
            var key = (int)ActiveSkills.EyeDropper;

            AddSpentActionPoint(key);
        }
    }
}
