using System.Collections.Generic;

namespace ScrumPlanningPoker.ViewModels
{
    public class ScrumMasterViewModel
    {
        public int UniqueKey { get; set; }
        public ICollection<Card> AvailableCards { get; set; }
    }
}