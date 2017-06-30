using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using ScrumPlanningPoker.Models;

namespace ScrumPlanningPoker.ViewModels
{
    public class HoofdschermViewModel
    {
        public string Error { get; set; }
        public Collection<Player> Spelers { get; set; }
        public int UniqueKey { get; set; }
    }
}