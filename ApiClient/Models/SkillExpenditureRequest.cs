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
            const int key = (int)PassiveSkills.Hypermutation;

            AddSpentPoint(key);
        }

        public void IncreaseRegeneration()
        {
            const int key = (int)PassiveSkills.Regeneration;

            AddSpentPoint(key);
        }

        public void IncreaseAntiApoptosis()
        {
            const int key = (int)PassiveSkills.AntiApoptosis;

            AddSpentPoint(key);
        }

        public void IncreaseBudding()
        {
            const int key = (int)PassiveSkills.Budding;

            AddSpentPoint(key);
        }

        public void IncreaseMycotoxicity()
        {
            var key = (int)PassiveSkills.Mycotoxicity;

            AddSpentPoint(key);
        }

        public void IncreaseHydrophilia()
        {
            const int key = (int)PassiveSkills.Hydrophilia;

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
            AddActiveCellChange(
                gridCellIndex, (int) ActiveSkills.EyeDropper);
        }

        public void AddDeadCell(int gridCellIndex)
        {
            AddActiveCellChange(
                gridCellIndex, (int)ActiveSkills.DeadCell);
        }

        public void AddActiveCellChange(int gridCellIndex, int activeSkillId)
        {
            if (ActiveSkillChanges.ContainsKey(activeSkillId))
            {
                ActiveSkillChanges[activeSkillId].ActiveCellChanges.Add(gridCellIndex);
            }
            else
            {
                var skillUpgrade = new ActiveSkillChanges();
                skillUpgrade.ActiveCellChanges.Add(gridCellIndex);
                ActiveSkillChanges.Add(activeSkillId, skillUpgrade);
            }
        }

        public void IncreaseSpores()
        {
            const int key = (int)PassiveSkills.Spores;

            AddSpentPoint(key);
        }

        public void UseEyeDropper()
        {
            const int key = (int)ActiveSkills.EyeDropper;

            AddSpentActionPoint(key);
        }

        public void UseDeadCell()
        {
            const int key = (int)ActiveSkills.DeadCell;

            AddSpentActionPoint(key);
        }
    }
}
