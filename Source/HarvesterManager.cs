using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KwarterMaster
{
    public class HarvesterManager : MonoBehaviour
    {
        private readonly List<HarvesterInfo> harversterInfos;

        public HarvesterManager()
        {
            harversterInfos = new List<HarvesterInfo>();
        }


        public void Update()
        {
            harversterInfos.Clear();


            //Debug.Log("Update harvester.");


            foreach (Part part in EditorLogic.fetch.ship.parts)
            {
                foreach (ModuleResourceHarvester harvester in part.Modules.OfType<ModuleResourceHarvester>())
                {
                    if (harvester.ResourceName == "Ore")
                    {
                        float oreOutput = harvester.Efficiency;
                        float powerConsumption = (float)harvester.eInput.Ratio;
                        string drillName = part.partInfo.title;            
                        //Debug.Log(drillName);
                        //Debug.Log(powerConsumption);
                        //Debug.Log(oreOutput);
                        harversterInfos.Add(new HarvesterInfo(drillName, oreOutput, powerConsumption));
                    }
                }
            }
        }

        public List<HarvesterInfo> GetHarvesters()
        {
            return harversterInfos;
        }
    }
}
