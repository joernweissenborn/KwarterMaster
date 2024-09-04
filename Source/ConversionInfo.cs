using System.Collections.Generic;

namespace KwarterMaster
{
    public class ConversionInfo
    {
        public string Name { get; set; }
        public string InputResource { get; set; }
        public float InputRate { get; set; }

        public string OutputResource { get; set; }
        public float OutputRate { get; set; }
        
        public float ECInput { get; set; }
    }

}