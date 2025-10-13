using RimWorld;
using Verse;
using DubsBadHygiene;
using System.Linq;

namespace DubsElectrolysis
{
    public class CompElectrolyzer : ThingComp
    {
        private CompProperties_Electrolyzer Props => (CompProperties_Electrolyzer)props;

        private CompPowerTrader powerComp;
        private CompPipe waterPipe;  // DBH water pipe
        private CompGasPipe oxygenPipe;  // Our custom gas pipe
        private CompGasPipe hydrogenPipe;  // Our custom gas pipe
        private CompFlickable flickable;

        public float h2Stored = 0f;
        public float o2Stored = 0f;

        private bool canOperate = false;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerComp = parent.GetComp<CompPowerTrader>();
            flickable = parent.GetComp<CompFlickable>();

            // Get DBH water pipe
            var pipes = parent.GetComps<CompPipe>();
            foreach (var pipe in pipes)
            {
                if (pipe.pipeNet?.pipeType == PipeType.Water)
                {
                    waterPipe = pipe;
                    break;
                }
            }

            // Get our custom gas pipes
            var gasPipes = parent.GetComps<CompGasPipe>();
            foreach (var gasPipe in gasPipes)
            {
                if (gasPipe.GasType == GasType.Oxygen)
                    oxygenPipe = gasPipe;
                else if (gasPipe.GasType == GasType.Hydrogen)
                    hydrogenPipe = gasPipe;
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!parent.IsHashIntervalTick(60))
                return;

            // Check if we can operate
            canOperate = powerComp != null && powerComp.PowerOn &&
                        (flickable == null || flickable.SwitchIsOn) &&
                        waterPipe != null;

            if (!canOperate)
                return;

            // Check if storage is full
            if (h2Stored >= Props.h2StorageCapacity && o2Stored >= Props.o2StorageCapacity)
            {
                canOperate = false;
                return;
            }

            // Consume water and produce gases
            float waterAvailable = waterPipe.pipeNet?.AvailableWater ?? 0f;
            if (waterAvailable >= Props.waterConsumptionPerTick)
            {
                // Consume water
                waterPipe.pipeNet.DrawWater(Props.waterConsumptionPerTick);

                // Produce H2 if storage not full
                if (h2Stored < Props.h2StorageCapacity)
                {
                    h2Stored += Props.h2ProductionPerTick;
                    if (h2Stored > Props.h2StorageCapacity)
                        h2Stored = Props.h2StorageCapacity;
                }

                // Produce O2 if storage not full
                if (o2Stored < Props.o2StorageCapacity)
                {
                    o2Stored += Props.o2ProductionPerTick;
                    if (o2Stored > Props.o2StorageCapacity)
                        o2Stored = Props.o2StorageCapacity;
                }
            }

            // Distribute gases to pipe networks
            DistributeGases();
        }

        private void DistributeGases()
        {
            // Distribute H2 to hydrogen gas network
            if (hydrogenPipe != null && hydrogenPipe.gasNet != null && h2Stored > 0f)
            {
                float transferred = hydrogenPipe.gasNet.AddGas(h2Stored);
                h2Stored -= transferred;
            }

            // Distribute O2 to oxygen gas network
            if (oxygenPipe != null && oxygenPipe.gasNet != null && o2Stored > 0f)
            {
                float transferred = oxygenPipe.gasNet.AddGas(o2Stored);
                o2Stored -= transferred;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref h2Stored, "h2Stored", 0f);
            Scribe_Values.Look(ref o2Stored, "o2Stored", 0f);
        }

        public override string CompInspectStringExtra()
        {
            string text = "";

            if (!canOperate)
            {
                if (powerComp == null || !powerComp.PowerOn)
                    text += "No power\n";
                else if (h2Stored >= Props.h2StorageCapacity && o2Stored >= Props.o2StorageCapacity)
                    text += "Storage full - shutting down\n";
                else if (waterPipe == null)
                    text += "No water connection\n";
            }
            else
            {
                text += "Operating\n";
            }

            text += $"H2 Storage: {h2Stored:F1}/{Props.h2StorageCapacity:F0}\n";
            text += $"O2 Storage: {o2Stored:F1}/{Props.o2StorageCapacity:F0}";

            return text;
        }
    }
}
