using Verse;
using DubsBadHygiene;

namespace DubsElectrolysis
{
    public class CompGasStorage : ThingComp
    {
        private CompProperties_GasStorage Props => (CompProperties_GasStorage)props;

        private CompPipe gasPipe;
        public float storedGas = 0f;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            // Get the appropriate pipe based on gas type
            var pipes = parent.GetComps<CompPipe>();
            foreach (var pipe in pipes)
            {
                if (Props.gasType == "Hydrogen" && pipe.pipeNet?.pipeType == PipeType.Hydrogen)
                {
                    gasPipe = pipe;
                    break;
                }
                else if (Props.gasType == "Oxygen" && pipe.pipeNet?.pipeType == PipeType.Oxygen)
                {
                    gasPipe = pipe;
                    break;
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!parent.IsHashIntervalTick(60))
                return;

            if (gasPipe == null || gasPipe.pipeNet == null)
                return;

            // Try to fill from network if not full
            if (storedGas < Props.storageCapacity)
            {
                float needed = Props.storageCapacity - storedGas;
                float received = 0f;

                if (Props.gasType == "Hydrogen")
                    received = gasPipe.pipeNet.DrawHydrogen(needed) ?? 0f;
                else if (Props.gasType == "Oxygen")
                    received = gasPipe.pipeNet.DrawOxygen(needed) ?? 0f;

                storedGas += received;
                if (storedGas > Props.storageCapacity)
                    storedGas = Props.storageCapacity;
            }

            // Provide gas back to network when needed
            if (storedGas > 0f)
            {
                // This allows the network to draw from storage tanks
                // The actual drawing is handled by the pipe network when consumers request gas
            }
        }

        public float DrawGas(float amount)
        {
            if (amount <= 0f)
                return 0f;

            float drawn = amount;
            if (drawn > storedGas)
                drawn = storedGas;

            storedGas -= drawn;
            return drawn;
        }

        public float AddGas(float amount)
        {
            if (amount <= 0f)
                return 0f;

            float space = Props.storageCapacity - storedGas;
            float added = amount;
            if (added > space)
                added = space;

            storedGas += added;
            return added;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref storedGas, "storedGas", 0f);
        }

        public override string CompInspectStringExtra()
        {
            float percentage = (storedGas / Props.storageCapacity) * 100f;
            return $"{Props.gasType} Storage: {storedGas:F1}/{Props.storageCapacity:F0} ({percentage:F0}%)";
        }
    }
}
