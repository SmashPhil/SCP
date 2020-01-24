using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace SCP
{
    class HediffComp_CureDisease : HediffComp
    {
        #region cheap hash interval stuff
        private int hashOffset = 0;
        public bool IsCheapIntervalTick(int interval) => (int)(Find.TickManager.TicksGame + hashOffset) % interval == 0;
        public override void CompPostMake()
        {
            base.CompPostMake();
            hashOffset = Pawn.thingIDNumber.HashOffset(); // this means all hediffs with this setup on a pawn will tick at the same time. Should be fine, but might want to change later.
        }
        #endregion cheap hash interval stuff

        HediffCompProperties_CureDisease Props => (HediffCompProperties_CureDisease)base.props;

        List<Hediff> Diseases => Pawn.health.hediffSet.hediffs.Where(x => x.def.everCurableByItem
                                                                       && x.def.isBad
                                                                       && x.def.HasComp(typeof(HediffComp_Immunizable))).ToList();
                                                                       // might also want to check whether it's an added part or smth in the future, but :blobshrug:
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (IsCheapIntervalTick(Props.ticksPerHeal))
            {
                foreach (Hediff h in Diseases)
                {
                    HediffComp_Immunizable hci;
                    if ((hci = h.TryGetComp<HediffComp_Immunizable>()) != null && !hci.FullyImmune)
                    {
                        base.Pawn.health.immunity.GetImmunityRecord(h.def).immunity = Mathf.Clamp01(hci.Immunity + Props.immunityIncrease);
                    }
                }
            }
        }

        public override void CompPostPostRemoved()
        {
            foreach(Hediff h in Diseases)
            {
                HediffComp_Immunizable hci;
                if((hci = h.TryGetComp<HediffComp_Immunizable>()) != null && !hci.FullyImmune)
                {
                    base.Pawn.health.immunity.GetImmunityRecord(h.def).immunity = 1f;
                }
            }
        }
    }
    public class HediffCompProperties_CureDisease : HediffCompProperties
    {
        public float immunityIncrease;
        public int ticksPerHeal;

        public HediffCompProperties_CureDisease()
        {
            base.compClass = typeof(HediffComp_CureDisease);
        }
    }
}
