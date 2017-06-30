using System.Collections.Generic;
using ScrumPlanningPoker.Enums;

namespace ScrumPlanningPoker.ViewModels
{
    public class ScrumMasterViewModel
    {
        public int UniqueKey { get; set; }
        public ICollection<Card> AvailableCards { get; set; }
    }
}