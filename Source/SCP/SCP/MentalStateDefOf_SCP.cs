using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace SCP
{
    [DefOf]
    public class MentalStateDefOf_SCP
    {
        static MentalStateDefOf_SCP()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MentalStateDefOf_SCP));
        }

        public static MentalStateDef FollowTheVoices;
    }
}
