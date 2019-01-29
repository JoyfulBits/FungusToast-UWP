using System.Collections.Generic;

namespace ApiClient.Models
{
    public class GrowthCycle
    {
        public List<ToastChange> ToastChanges { get; set; }
        public Dictionary<string, int> MutationPointsEarned { get; set; }
    }
}