using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace SCP
{
    public class GameCondition_SCP939 : GameCondition
    {
        public override void Init()
        {
            base.Init();
            scp939_GeneratedTotal = 0;
            scp939Generated = new List<Pawn>();
        }

        private bool TimeValid
        {
            get
            {
                int hour = GenLocalDate.HourOfDay(SingleMap);
                return (hour >= 20 || hour < 4);
            }
        }

        private bool CanProbeMap
        {
            get
            {
                return TimeValid && scp939Generated.Count > 0; //Edit later for probing defenses or ambushes
            }
        }

        private int HoursTillDawn
        {
            get
            {
                int hour = GenLocalDate.HourOfDay(SingleMap);
                if(hour <= 6)
                    return 6 - hour;
                return 24 - hour + 6;
            }
        }

        public List<Pawn> ActiveSCPInArea => scp939Generated;

        public void AddToRegistry(Pawn p)
        {
            if(p.IsWorldPawn())
            {
                Log.Error("Tried caching " + p.LabelShort + " while it is already a world pawn. This should not happen. - Smash Phil");
                return;
            }
            if(p.Spawned)
                p.DeSpawn();
            if(!scp939Generated.Contains(p))
                scp939Generated.Add(p);
            Find.WorldPawns.PassToWorld(p, PawnDiscardDecideMode.Decide);
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();
            if(Find.TickManager.TicksGame % TickCheckSpawn == 0)
            {
                if(CanProbeMap)
                {
                    if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 cell, SingleMap, CellFinder.EdgeRoadChance_Animal, false, null))
                        return;
                    Pawn scp = scp939Generated.Pop();
                    GenSpawn.Spawn(scp, cell, SingleMap, WipeMode.Vanish);
                    Log.Message("SCP SPAWNED - Left: " + scp939Generated.Count);
                }
                if(scp939_GeneratedTotal < scp939Count)
                {
                    Pawn pawn = null;
                    PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf_SCP.SCP_939_PawnKindDef, null, PawnGenerationContext.NonPlayer, SingleMap.Tile, false, false, false, false, false, true,
                        0f, false, false, false, false, false, false, false, true, 0f, null, 0f, null, null, null, null, null, null, null, null, null, null, null, null);
                    pawn = PawnGenerator.GeneratePawn(request);
                    scp939_GeneratedTotal++;
                    scp939Generated.Add(pawn);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref scp939Count, "scp939Count");
            Scribe_Values.Look(ref scp939_GeneratedTotal, "scp939_GeneratedTotal");
            Scribe_Collections.Look(ref scp939Generated, "scp939Generated", LookMode.Reference);
        }

        public int scp939Count;

        private const int TickCheckSpawn = 1000;

        private const int TickGeneratePawn = 2000;

        private int scp939_GeneratedTotal;

        private List<Pawn> scp939Generated = new List<Pawn>();
    }
}
