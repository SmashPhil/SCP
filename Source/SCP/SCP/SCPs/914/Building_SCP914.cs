using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace SCP
{
    class Building_SCP914 : Building
    {
        enum Setting
        {
            Rough,
            Coarse,
            OneToOne,
            Fine,
            VeryFine
        }

        Setting setting;

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos()) yield return g;
            //yield return new Gizmo_914Setting
        }

        public List<Thing> Process(Thing input)
        {
            if(input is Pawn s)
            {

            }
            if (input.def.IsStuff)
            {
                switch (setting)
                {
                    case Setting.Rough:
                    case Setting.Coarse:
                    case Setting.OneToOne:
                    case Setting.Fine:
                    case Setting.VeryFine:
                        break;
                }
            }
            return new List<Thing>();
        }
        public bool ShouldAccept(Thing input)
        {
            return input is Pawn ||                                                       // Pawns
                   input.def.IsStuff ||                                                   // Stuff
                   (input.GetInnerIfMinified().def.thingClass == typeof(Building_Art)) || // Art
                   input.def.Verbs.Any() ||                                               // Weapons
                   input is Corpse;                                                       // Corpses
                   // food?
        }
    }
}
