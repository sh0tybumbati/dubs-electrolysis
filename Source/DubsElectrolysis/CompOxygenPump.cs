using RimWorld;
using Verse;
using DubsBadHygiene;

namespace DubsElectrolysis
{
    public class CompOxygenPump : ThingComp
    {
        private CompProperties_OxygenPump Props => (CompProperties_OxygenPump)props;

        private CompPowerTrader powerComp;
        private CompPipe oxygenPipe;
        private CompFlickable flickable;

        private bool isOperating = false;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerComp = parent.GetComp<CompPowerTrader>();
            flickable = parent.GetComp<CompFlickable>();

            // Get oxygen pipe
            var pipes = parent.GetComps<CompPipe>();
            foreach (var pipe in pipes)
            {
                if (pipe.pipeNet?.pipeType == PipeType.Oxygen)
                {
                    oxygenPipe = pipe;
                    break;
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!parent.IsHashIntervalTick(60))
                return;

            UpdateOperation();
        }

        private void UpdateOperation()
        {
            // Check if we can operate
            bool canOperate = powerComp != null && powerComp.PowerOn &&
                            (flickable == null || flickable.SwitchIsOn) &&
                            oxygenPipe != null;

            if (canOperate)
            {
                // Try to draw oxygen from pipe network
                float needed = Props.o2ConsumptionPerTick;
                float received = oxygenPipe.pipeNet?.DrawOxygen(needed) ?? 0f;

                if (received > 0f)
                {
                    // Oxygenate the room
                    Room room = parent.GetRoom(RegionType.Set_Passable);
                    if (room != null && !room.PsychologicallyOutdoors)
                    {
                        // For Odyssey DLC integration: provide oxygen to room
                        // This would integrate with the gravship oxygen system
                        // Placeholder for actual Odyssey integration
                        isOperating = true;
                    }
                    else
                    {
                        isOperating = false;
                    }
                }
                else
                {
                    isOperating = false;
                }
            }
            else
            {
                isOperating = false;
            }
        }

        public override string CompInspectStringExtra()
        {
            if (!isOperating)
            {
                if (powerComp == null || !powerComp.PowerOn)
                    return "No power";
                else if (flickable != null && !flickable.SwitchIsOn)
                    return "Switched off";
                else if (oxygenPipe == null)
                    return "No oxygen connection";
                else
                    return "No oxygen available";
            }

            return "Providing oxygen";
        }
    }
}
