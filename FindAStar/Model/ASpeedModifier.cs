using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindAStar.Model
{
    public class ASpeedModifier
    {
        public int TimePassedForSearch { get; set; }
        public int TimePassedForView { get; set; }

        public int SearchDelayInMilliseconds { get; set; }
        public int ViewDelayInMilliseconds { get; set; }

        public int GetTime()
        {
            return (TimePassedForSearch + TimePassedForView) / 2;
        }
    }
}
