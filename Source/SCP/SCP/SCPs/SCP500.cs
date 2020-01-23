using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace SCP
{
    class HediffComp_CureDisease : HediffComp
    {

        public override void CompPostTick(ref float severityAdjustment)
        {
            // tick: reduce severity of all diseases
        }

        public override void CompPostPostRemoved()
        {
            // remove all remaining diseases if not cured
        }
    }
    public class HediffCompProperties_CureDisease : HediffCompProperties
    {
        public float severityReduction;
        public int ticks;

        public HediffCompProperties_CureDisease()
        {
            base.compClass = typeof(HediffComp_CureDisease);
        }
    }
}
