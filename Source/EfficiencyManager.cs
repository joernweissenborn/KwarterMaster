using System.Collections.Generic;

namespace KwarterMaster
{
	public class EfficiencyManager
	{
		public bool IsEngineerOnBoard { get; set; }
		public int EngineerStars { get; set; }
		public float Multiplier
		{
			get
			{
				float efficiencyMultiplier = 0.05f;
				if (IsEngineerOnBoard)
				{
					efficiencyMultiplier += efficiencyMultiplier * 4 * (EngineerStars + 1);
				}
				return efficiencyMultiplier;
			}
		}

		public EfficiencyManager()
		{
			IsEngineerOnBoard = false;
			EngineerStars = 5;
		}
	}
}