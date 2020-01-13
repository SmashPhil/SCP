using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using Verse.AI;
using RimWorld;
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
                    __result *= 10;
                else if(__instance.GetComp<CompVoices>().TargetLured)
                    __result *= 10000;
            }
        }

        public static bool SCP939_DontWarnPlayerHunted(JobDriver_PredatorHunt __instance)
        {
            return __instance.pawn.GetComp<CompVoices>() is null;
        }

        #endregion 939
    }
}
