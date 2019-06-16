using System.Collections.Generic;

namespace ApiClient.Models
{
    public class SkillsList
    {
        public List<PassiveSkill> PassiveSkills { get; set; }
        public List<ActiveSkill> ActiveSkills { get; set; }
    }
}
