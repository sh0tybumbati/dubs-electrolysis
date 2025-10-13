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
            string text = "";

            // Status and power output
            if (isGenerating)
            {
                text += "Power output: " + (powerPlant?.Props.PowerConsumption * -1).ToString("F0") + " W\n";

                // Oxygen consumption rate
                float consumptionPerDay = Props.o2ConsumptionPerTick * 60000f; // ticks per day
                text += $"O2 consumption: {consumptionPerDay:F1}/day\n";

                // Efficiency note
                text += "Efficiency: 2x vs chemfuel";
            }
            else
            {
                if (flickable != null && !flickable.SwitchIsOn)
                    text += "Switched off";
                else if (breakdownable != null && breakdownable.BrokenDown)
                    text += "Broken down";
                else if (oxygenPipe == null)
                    text += "No oxygen connection";
                else
                    text += "No oxygen available";
            }

            return text.TrimEnd('\n');
        }
    }
}
