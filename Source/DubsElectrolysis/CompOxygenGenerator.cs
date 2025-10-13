using RimWorld;
using Verse;
using DubsBadHygiene;

namespace DubsElectrolysis
{
    public class CompOxygenGenerator : ThingComp
    {
        private CompProperties_OxygenGenerator Props => (CompProperties_OxygenGenerator)props;

        private CompPowerPlant powerPlant;
        private CompPipe oxygenPipe;
        private CompFlickable flickable;
        private CompBreakdownable breakdownable;

        private bool isGenerating = false;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerPlant = parent.GetComp<CompPowerPlant>();
            flickable = parent.GetComp<CompFlickable>();
            breakdownable = parent.GetComp<CompBreakdownable>();

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

            UpdatePowerGeneration();
        }

        private void UpdatePowerGeneration()
        {
            // Check if we can generate power
            bool canGenerate = (flickable == null || flickable.SwitchIsOn) &&
                             (breakdownable == null || !breakdownable.BrokenDown) &&
                             oxygenPipe != null;

            if (canGenerate)
            {
                // Try to consume oxygen from pipe network
                float needed = Props.o2ConsumptionPerTick;
                float consumed = oxygenPipe.pipeNet?.DrawOxygen(needed) ?? 0f;

                if (consumed > 0f)
                {
                    isGenerating = true;
                    // Power output is handled by CompPowerPlant
                    // Heat is handled by CompHeatPusher
                }
                else
                {
                    isGenerating = false;
                }
            }
            else
            {
                isGenerating = false;
            }

            // Update power output based on generation status
            if (powerPlant != null)
            {
                // Power output is configured in XML, just ensure it's active when generating
                powerPlant.PowerOutput = isGenerating ? powerPlant.Props.PowerConsumption : 0f;
            }
        }

        public override string CompInspectStringExtra()
        {
            if (!isGenerating)
            {
                if (flickable != null && !flickable.SwitchIsOn)
                    return "Switched off";
                else if (breakdownable != null && breakdownable.BrokenDown)
                    return "Broken down";
                else if (oxygenPipe == null)
                    return "No oxygen connection";
                else
                    return "No oxygen available";
            }

            return "Burning oxygen - generating power";
        }
    }
}
