using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace SCP
{
    public class MentalState_Voices : MentalState
    {
        public override void MentalStateTick()
        {
            base.MentalStateTick();
            if(this.pawn.Position == voicesHeardFrom)
                this.RecoverFromState();
        }

        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Quiet;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.voicesHeardFrom, "voicesHeardFrom", default(IntVec3), false);
        }

        public IntVec3 voicesHeardFrom;
    }
}
