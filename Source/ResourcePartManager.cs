
using System;
using System.Linq;
using UnityEngine;

namespace KwarterMaster
{
    public class ResourcePartManager
    {
        private EfficiencyManager _efficiencyManager;
        private ResourceFlowGraph _resourceFlowGraph;

        public ResourcePartManager(EfficiencyManager efficiencyManager, ResourceFlowGraph resourceFlowGraph)
        {
            _efficiencyManager = efficiencyManager;
            _resourceFlowGraph = resourceFlowGraph;
        }

        private void VisitParts(Action<Part> action)
        {
            foreach (Part part in EditorLogic.fetch.ship.parts)
            {
                action(part);
            }
        }

        private void VisitModules<T>(Action<T> action) where T : PartModule
        {
            VisitParts(part =>
            {
                foreach (T module in part.Modules.OfType<T>())
                {
                    action(module);
                }
            });
        }

        public void GetParts()
        {
            GetHarvesters();
            GetConverters();
            GetStorage();
        }

        public void GetHarvesters()
        {
            VisitModules<ModuleResourceHarvester>(harvester =>
            {
                float output = harvester.Efficiency * _efficiencyManager.Multiplier;
                float powerConsumption = (float)harvester.eInput.Ratio * _efficiencyManager.Multiplier;
                string resourceName = harvester.ResourceName;
                _resourceFlowGraph.AddHarvester(resourceName, output, powerConsumption);
            });
        }

        public void GetConverters()
        {
            VisitModules<ModuleResourceConverter>(converter =>
            {
                Debug.Log("Converter found: " + converter.part.partInfo.title);
                Debug.Log("active: " + converter.IsActivated);
                Debug.Log("from, to: " + converter.inputList[0].ResourceName + " -> " + converter.outputList[0].ResourceName);
                if (!converter.IsActivated)
                {
                    return;
                }

                bool hasMultyInput = converter.inputList.Count > 2;

                if (hasMultyInput)
                {
                    Debug.LogWarning("Converter has multiple inputs. This is not supported.");
                    return;
                }

                string inputResource = "";
                float inputRate = 0;
                float powerConsumption = 0;
                float ratio = _efficiencyManager.Multiplier / converter.outputList.Count;
                foreach (var input in converter.inputList)
                {
                    Debug.Log("Input: " + input.ResourceName + " " + input.Ratio);
                    if (input.ResourceName == "ElectricCharge")
                    {
                        powerConsumption = (float)(input.Ratio * ratio);
                    }
                    else
                    {
                        inputResource = input.ResourceName;
                        inputRate = (float)(input.Ratio * ratio);
                    }
                }
                foreach (var output in converter.outputList)
                {
                    Debug.Log("Output: " + output.ResourceName + " " + output.Ratio + "Input: " + inputResource + " " + inputRate);
                    _resourceFlowGraph.AddFlow(inputResource, output.ResourceName, inputRate, (float)(output.Ratio * _efficiencyManager.Multiplier), powerConsumption);
                }
            });
        }

        public void GetStorage()
        {
            VisitParts(part =>
            {
                foreach (PartResource resource in part.Resources)
                {
                    Debug.Log("Resource: " + resource.resourceName + " " + resource.maxAmount);
                    _resourceFlowGraph.AddStorage(resource.resourceName, (float)resource.maxAmount);
                }
            });
        }
    }
}