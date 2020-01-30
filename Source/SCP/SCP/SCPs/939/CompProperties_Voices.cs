using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace SCP
{
    public class CompProperties_Voices : CompProperties
    {
        public CompProperties_Voices()
        {
            this.compClass = typeof(CompVoices);
        }

        public int voiceRange;
        public int directAttackRange;
        public int startPounceOnLuredTargetRange;
    }
}
