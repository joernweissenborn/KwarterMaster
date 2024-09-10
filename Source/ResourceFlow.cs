using System;

namespace KwarterMaster
{
    public class ResourceFlow
    {
        public ResourceNode Input { get; private set; }
        public ResourceNode Output { get; private set; }
        public float Rate { get; set; }
        public float ECUsage { get; set; }

        public ResourceFlow(ResourceNode input, ResourceNode output, float rate, float ecUsage)
        {
            Input = input;
            Output = output;
            Rate = rate;
            ECUsage = ecUsage;
        }

        public ResourceFlow(ResourceNode output, float rate, float ecUsage)
        {
            Input = null;
            Output = output;
            Rate = rate;
            ECUsage = ecUsage;
        }
        public bool IsHarvester()
        {
            return Input == null;
        }

        public override string ToString()
        {
            return $"{Input?.Name ?? "Harvester"} -> {Output.Name} ({Rate} ec: {ECUsage})";
        }


    }

}