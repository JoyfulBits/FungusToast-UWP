using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApiClient.Models
{
    public class SkillExpenditureRequest
    {
        public List<SkillUpgrade> SkillUpgrades =>
            new List<SkillUpgrade>
            {
                new SkillUpgrade
                {
                    Id = Skills.Hypermutation,
                    PointsSpent = HypermutationPoints
                },
                new SkillUpgrade
                {
                    Id = Skills.Regeneration,
                    PointsSpent = RegenerationPoints
                },
                new SkillUpgrade
                {
                    Id = Skills.AntiApoptosis,
                    PointsSpent = AntiApoptosisPoints
                },
                new SkillUpgrade
                {
                    Id = Skills.Budding,
                    PointsSpent = BuddingPoints
                },
                new SkillUpgrade
                {
                    Id = Skills.Mycotoxicity,
                    PointsSpent = MycotoxicityPoints
                }
            };

        [JsonIgnore]
        public int HypermutationPoints { get; set; }
        [JsonIgnore]
        public int BuddingPoints { get; set; }
        [JsonIgnore]
        public int AntiApoptosisPoints { get; set; }
        [JsonIgnore]
        public int RegenerationPoints { get; set; }
        [JsonIgnore]
        public int MycotoxicityPoints { get; set; }
        public SkillExpenditure SkillExpenditure { get; }

        public SkillExpenditureRequest(SkillExpenditure skillExpenditure)
        {
            SkillExpenditure = skillExpenditure;
        }
    }
}
