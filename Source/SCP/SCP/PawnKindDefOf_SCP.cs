using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace SCP
{
    [DefOf]
    public static class PawnKindDefOf_SCP
    {
        static PawnKindDefOf_SCP()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PawnKindDefOf_SCP));
        }

        public static PawnKindDef SCP_939_PawnKindDef;
    }
}
