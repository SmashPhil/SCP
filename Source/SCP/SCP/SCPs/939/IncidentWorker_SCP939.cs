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
            if(GenLocalDate.HourOfDay(map) < 19 && GenLocalDate.HourOfDay(map) > 5)
                return false;
            return RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 intVec, map, CellFinder.EdgeRoadChance_Animal, false, null);
        }

        private void ResolveArrivalPoints(IncidentParms parms)
        {
            if(parms.points <= 0f)
            {
                Log.Error("RaidEnemy is resolving raid points. They should always be set before initiating the incident.", false);
                parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);
            }
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            this.ResolveArrivalPoints(parms);
            Map map = (Map)parms.target;
            PawnKindDef scp939 = PawnKindDefOf_SCP.SCP_939_PawnKindDef;
            if(scp939 is null)
                return false;
            float num = parms.points;
            int num2 = GenMath.RoundRandom(num / scp939.combatPower);
            int numMax = Rand.RangeInclusive(15, 20);
            num2 = Mathf.Clamp(num2, 1, numMax);
            int num3 = HoursTillDawn(map) * 2500;
            if(!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 intVec, map, CellFinder.EdgeRoadChance_Animal, false, null))
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
        
        private int HoursTillDawn(Map map)
        {
            int hour = GenLocalDate.HourOfDay(map);
            if(hour <= 6)
                return 6 - hour;
            return 24 - hour + 6;
        }
    }
}
