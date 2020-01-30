using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using Verse.AI;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace SCP
{
    [StaticConstructorOnStartup]
    internal static class SCPHarmony
    {
        static SCPHarmony()
        {
            var harmony = HarmonyInstance.Create("rimworld.scp.smashphil");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            //HarmonyInstance.DEBUG = true;

            #region Functions

            //SCP939
            harmony.Patch(original: AccessTools.Method(type: typeof(Need_Food), name: nameof(Need_Food.NeedInterval)), prefix: null,
                postfix: new HarmonyMethod(type: typeof(SCPHarmony),
                name: nameof(SCP939_Starving)));
            harmony.Patch(original: AccessTools.Method(type: typeof(FoodUtility), name: nameof(FoodUtility.IsAcceptablePreyFor)),
                prefix: new HarmonyMethod(type: typeof(SCPHarmony),
                name: nameof(SCP939_HumansOnlyAcceptablePrey)));
            harmony.Patch(original: AccessTools.Method(type: typeof(Pawn), name: "TicksPerMove"), prefix: null,
                postfix: new HarmonyMethod(type: typeof(SCPHarmony),
                name: nameof(SCP939_VoicesMovementSpeed)));
            harmony.Patch(original: AccessTools.Method(type: typeof(JobDriver_PredatorHunt), name: "CheckWarnPlayer"),
                prefix: new HarmonyMethod(type: typeof(SCPHarmony),
                name: nameof(SCP939_DontWarnPlayerHunted)));
            harmony.Patch(original: AccessTools.Method(type: typeof(Pawn), name: nameof(Pawn.TickRare)), prefix: null,
                postfix: new HarmonyMethod(type: typeof(SCPHarmony),
                name: nameof(TickMindstateLeaveDaylight)));
            harmony.Patch(original: AccessTools.Method(type: typeof(WorldPawns), name: nameof(WorldPawns.GetSituation)), prefix: null,
                postfix: new HarmonyMethod(type: typeof(SCPHarmony),
                name: nameof(SituationSCPEvent)));
            #endregion Functions
        }

        #region 939

        public static void SCP939_Starving(Need_Food __instance, Pawn ___pawn)
        {
           if(___pawn.def.defName == "SCP939_A")
            {
                __instance.CurLevel = 0.1f;
            }
        }

        public static bool SCP939_HumansOnlyAcceptablePrey(Pawn predator, Pawn prey, ref bool __result)
        {
            if (predator.def.defName == "SCP939_A")
            {
                __result = false;
                if (prey.RaceProps.Humanlike)
                {
                    __result = true;
                }
                return false;
            }
            return true;
        }

        public static void SCP939_VoicesMovementSpeed(bool diagonal, Pawn __instance, ref int __result)
        {
            if(__instance.TryGetComp<CompVoices>() != null)
            {
                if(__instance.GetComp<CompVoices>().VoicesActive)
                    __result *= 100;
                else if(__instance.GetComp<CompVoices>().TargetLured)
                    __result *= 10000;
            }
        }

        public static bool SCP939_DontWarnPlayerHunted(JobDriver_PredatorHunt __instance)
        {
            return __instance.pawn.GetComp<CompVoices>() is null;
        }

        public static void TickMindstateLeaveDaylight(Pawn __instance)
        {
            if(__instance?.kindDef == PawnKindDefOf_SCP.SCP_939_PawnKindDef && __instance.Spawned)
            {
                if(GenLocalDate.HourOfDay(__instance.Map) >= 5 && GenLocalDate.HourOfDay(__instance.Map) < 19)
                {
                    if(CellFinder.TryFindRandomPawnExitCell(__instance, out IntVec3 cell))
                    {
                        Job job = new Job(JobDefOf_SCP.LeaveMapDaylight, cell);
                        __instance.jobs.TryTakeOrderedJob(job, JobTag.DraftedOrder);
                    }
                }
            }
        }

        public static void SituationSCPEvent(Pawn p, ref WorldPawnSituation __result, WorldPawns __instance)
        {
            if(__result == WorldPawnSituation.Free && p.kindDef == PawnKindDefOf_SCP.SCP_939_PawnKindDef)
            {
                foreach(Map map in Find.Maps)
                {
                    if(map.GameConditionManager.ActiveConditions.Any(x => x is GameCondition_SCP939 && (x as GameCondition_SCP939).ActiveSCPInArea.Contains(p)))
                    {
                        __result = WorldPawnSituation.InTravelingTransportPod;
                        return;
                    }
                }
            }
        }

        #endregion 939
    }
}
