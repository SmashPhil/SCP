using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace SCP
{
    [DefOf]
    public static class JobDefOf_SCP
    {
        
        static JobDefOf_SCP()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf_SCP));
        }

        public static JobDef LeaveMapDaylight;
    }
}
