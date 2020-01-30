using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;

namespace SCP
{
    public class JobDriver_LeaveMapPromptly : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    GameCondition_SCP939 scpEvent = (GameCondition_SCP939)pawn.Map.GameConditionManager.ActiveConditions.Find(x => x is GameCondition_SCP939);
                    scpEvent.AddToRegistry(pawn);
                    return;
                }
            };
            yield break;
        }
    }
}
