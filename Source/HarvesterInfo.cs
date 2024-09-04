namespace KwarterMaster
{
    public class HarvesterInfo
    {
        public string Name { get; set; }
        public float MaxOre { get; set; }
        public float MaxEC { get; set; }

        public HarvesterInfo(string name, float maxOre, float maxEC)
        {
            Name = name;
            MaxOre = maxOre;
            MaxEC = maxEC;
        }
    }
}