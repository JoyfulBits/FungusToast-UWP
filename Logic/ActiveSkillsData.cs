using System.Collections.Generic;

namespace Logic
{
    public class ActiveSkillsData
    {
        public Dictionary<int, ActiveSkillData> SkillIdToActiveSkillData { get; }

        public ActiveSkillsData(Dictionary<int, ActiveSkillData> skillIdToActiveSkillData)//int waterDropletsPerEyeDropperPoint, int numberOfDeadCellsPerDeadCellAction)
        {
            SkillIdToActiveSkillData = skillIdToActiveSkillData;
        }

        public string GetActiveSkillMessage(int activeSkillId)
        {
            return SkillIdToActiveSkillData[activeSkillId].Message;
        }

        public int GetNumberOfActionsPerActionPoint(int activeSkillId)
        {
            return SkillIdToActiveSkillData[activeSkillId].ActionsPerActionPoint;
        }

        public int GetMinimumRound(int activeSkillId)
        {
            return SkillIdToActiveSkillData[activeSkillId].MinimumRound;
        }
    }
}