using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace KwarterMaster
{
    public class ConversionManager
    {
        private readonly List<ConversionInfo> converterInfos;

        public ConversionManager()
        {
            converterInfos = new List<ConversionInfo>();
        }

        public void Update()
        {
            converterInfos.Clear();

            foreach (Part part in EditorLogic.fetch.ship.parts)
            {
                foreach (ModuleResourceConverter converter in part.Modules.OfType<ModuleResourceConverter>())
                {
                    Debug.Log("Converter found: " + converter.part.partInfo.title);
                    Debug.Log("active: " + converter.IsActivated);
                    Debug.Log("status: " + converter.status);

                    if (!converter.IsActivated)
                    {
                        continue;
                    }

                    ConversionInfo converterInfo = new ConversionInfo
                    {
                        Name = converter.part.partInfo.title
                    };

                    foreach (var input in converter.inputList)
                    {
                        Debug.Log("Input: " + input.ResourceName + " " + input.Ratio);
                        if (input.ResourceName == "ElectricCharge")
                        {
                            converterInfo.ECInput = (float)input.Ratio;
                        }
                        else
                        {
                            converterInfo.InputResource = input.ResourceName;
                            converterInfo.InputRate = (float)input.Ratio;
                        }
                    }

                    foreach (var output in converter.outputList)
                    {
                        Debug.Log("Output: " + output.ResourceName + " " + output.Ratio);
                        converterInfo.OutputResource = output.ResourceName;
                    }

                    converterInfos.Add(converterInfo);
                }
            }
        }

        public List<ConversionInfo> GetConverters()
        {
            return converterInfos;
        }
    }
}