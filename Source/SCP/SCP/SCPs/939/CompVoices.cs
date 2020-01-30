using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using SPExtended;
using UnityEngine;
using Harmony;

namespace SCP
{
    public class CompVoices : ThingComp
    {
        private int voicesCycle = 0;
        private bool targetLured;
        public bool TargetLured => targetHunted != null && !targetHunted.Downed && !targetHunted.Dead && targetLured;
        public bool VoicesActive { get; private set; }
        public Pawn Pawn => parent as Pawn;
        public CompProperties_Voices Props => (CompProperties_Voices)this.props;

        private Pawn targetHunted;

        private SPTuples.SPTuple<Pawn, bool, Pawn> voiceAttempted;
        private bool WithinVoiceRange(IntVec3 targetPos)
        {
            return SPExtra.Distance(this.Pawn.Position, targetPos) <= this.Props.voiceRange;
        }

        private bool TooCloseToVoice()
        {
            if(targetHunted is null)
            {
                this.ResetVoices();
                return true;
            }
            else if(targetHunted.Downed || targetHunted.Dead)
            {
                return true;
            }

            return targetHunted.CanSee(this.Pawn) && this.WithinVoiceRange(this.targetHunted.Position) && SPExtra.Distance(this.Pawn.Position, this.targetHunted.Position) <= this.Props.directAttackRange;
        }

        private bool TooFarToVoice()
        {
            if(targetHunted is null)
            {
                return true;
            }
            else if(targetHunted.Downed || targetHunted.Dead)
            {
                return true;
            }
            return !this.WithinVoiceRange(this.targetHunted.Position);
        }

        public override void CompTick()
        {
            base.CompTick();
            if(this.TargetLured)
            {
                if(SPExtra.Distance(this.Pawn.Position, targetHunted.Position) <= this.Props.directAttackRange)
                {
                    this.targetLured = false;
                }
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            
            if(this.Pawn.CurJobDef == JobDefOf.PredatorHunt && this.WithinVoiceRange(this.Pawn.CurJob.targetA.Thing.Position) && !this.VoicesActive && voiceAttempted is null && !this.TargetLured)
            {
                targetHunted = this.Pawn?.CurJob?.targetA.Thing as Pawn;
                if(this.TooCloseToVoice())
                {
                    goto Block_TooClose;
                }

                this.VoicesActive = true;
                List<Pawn> pawnsToMimic = new List<Pawn>();
                foreach (Pawn p in this.targetHunted.MapHeld.mapPawns.AllPawnsSpawned)
                {
                    if (p.RaceProps.Humanlike && p != targetHunted && targetHunted.relations.everSeenByPlayer)
                    {
                        pawnsToMimic.Add(p);
                    }
                }

                Pawn randomVoice = pawnsToMimic.Count > 0 ? pawnsToMimic.RandomElement() : null;
                voiceAttempted = new SPTuples.SPTuple<Pawn, bool, Pawn>(targetHunted, false, randomVoice);
            }

            if(this.VoicesActive && voiceAttempted?.First != null && !voiceAttempted.Second)
            {
                if(this.TooFarToVoice())
                {
                    this.ResetVoices();
                    return;
                }

                voicesCycle++;
                if(voicesCycle >= 4)
                {
                    this.VoicesActive = false;
                    voiceAttempted.Second = true;
                    
                    int chanceToLure = (int)Mathf.Lerp(0f, 50f, ( (voiceAttempted.Third != null ? this.targetHunted.relations.OpinionOf(voiceAttempted.Third) : 0) + 100) / 200f);
                    //Log.Message(chanceToLure + "%");
                    //chanceToLure *= 999;
                    if(chanceToLure > Rand.Range(0, 100))
                    {
                        this.targetLured = true;
                        this.targetHunted.jobs.TryTakeOrderedJob(new Job(JobDefOf.GotoWander, this.Pawn.Position), JobTag.InMentalState);
                        this.StartMentalStateSpecificPos(this.targetHunted, this.Pawn.Position);
                    }
                }
            }
            Block_TooClose:;

            if (this.targetHunted != null && (this.targetHunted.Dead || this.targetHunted != this.Pawn?.CurJob?.targetA.Thing as Pawn) )
            {
                this.ResetVoices();
            }
        }

        private void StartMentalStateSpecificPos(Pawn pawn, IntVec3 position)
        {
            MentalState_Voices mstate = (MentalState_Voices)Activator.CreateInstance(MentalStateDefOf_SCP.FollowTheVoices.stateClass);
            mstate.voicesHeardFrom = position;
            mstate.pawn = pawn;
            mstate.def = MentalStateDefOf_SCP.FollowTheVoices;
            mstate.causedByMood = false;
            mstate.PreStart();
            if(pawn.Drafted)
                pawn.drafter.Drafted = false;
            AccessTools.Field(type: typeof(MentalStateHandler), name: "curStateInt").SetValue(pawn.mindState.mentalStateHandler, mstate);
        }

        private void ResetVoices()
        {
            this.VoicesActive = false;
            this.voicesCycle = 0;
            this.voiceAttempted = null;
            this.targetHunted = null;
            this.targetLured = false;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.voicesCycle, "voicesCycle", 0);
        }
    }
}
