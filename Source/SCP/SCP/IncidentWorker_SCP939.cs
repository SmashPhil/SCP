using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace SCP
{
    public class IncidentWorker_SCP939 : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if(!base.CanFireNowSub(parms))
                return false;
            Map map = (Map)parms.target;
            return RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 intVec, map, CellFinder.EdgeRoadChance_Animal, false, null);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            PawnKindDef scp939 = PawnKindDefOf_SCP.SCP_939_PawnKindDef;
            if(scp939 is null)
                return false;
            float num = StorytellerUtility.DefaultThreatPointsNow(map);
            int num2 = GenMath.RoundRandom(num / scp939.combatPower);
            int numMax = Rand.RangeInclusive(15, 20);
            num2 = Mathf.Clamp(num2, 1, numMax);
            int num3 = Rand.RangeInclusive(40000, 60000);
            IntVec3 intVec;
            if(!RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal, false, null))
                return false;
            Pawn pawn = null;
            for (int i = 0; i < num2; i++)
            {
                pawn = PawnGenerator.GeneratePawn(scp939, null);
                GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(intVec, map, 12, null), map, WipeMode.Vanish);
                pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num3;
            }
            Find.LetterStack.ReceiveLetter("LetterSCP939Enters".Translate().CapitalizeFirst(), "LetterSCP939EntersText".Translate(), LetterDefOf.ThreatBig, pawn, null, null);
            return true;
        }
    }
}
