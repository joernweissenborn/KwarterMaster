using System.Collections.Generic;

namespace KwarterMaster
{
	public class EfficiencyManager
	{
		public bool IsEngineerOnBoard { get; set; }
		public int EngineerStars { get; set; }
		public int ResourceConcentration { get; set; }

		public EfficiencyManager(bool isEngineerOnBoard, int engineerStars, int resourceConcentration)
		{
			IsEngineerOnBoard = isEngineerOnBoard;
			EngineerStars = engineerStars;
			ResourceConcentration = resourceConcentration;
		}

		public float CalculateEfficiencyMultiplier()
		{
			float efficiencyMultiplier = 0.05f;
			if (IsEngineerOnBoard)
			{
				efficiencyMultiplier += efficiencyMultiplier * 4 * (EngineerStars + 1);
			}
			return efficiencyMultiplier;
		}

		public float CalculateOreOutput(HarvesterInfo harvesterInfo)
		{
			float baseOutput = harvesterInfo.MaxOre * (ResourceConcentration / 100.0f);
			float efficiencyMultiplier = CalculateEfficiencyMultiplier();
			return baseOutput * efficiencyMultiplier;
		}

		public float CalculateECUsage(HarvesterInfo harvesterInfo)
		{
			float baseECUsage = harvesterInfo.MaxEC * (ResourceConcentration / 100.0f);
			float efficiencyMultiplier = CalculateEfficiencyMultiplier();
			return baseECUsage * efficiencyMultiplier;
		}
		
		public List<string[]> GenerateHarvesterTableData(List<HarvesterInfo> harvesters)
		{
			var tableData = new List<string[]>();

			foreach (var harvester in harvesters)
			{
				float currentOreOutput = CalculateOreOutput(harvester);
				float currentECUsage = CalculateECUsage(harvester);

				var row = new string[]
				{
					harvester.Name,
					$"{currentOreOutput:F3}/{harvester.MaxOre:F3}",
					$"{currentECUsage:F3}/{harvester.MaxEC:F3}"
				};

				tableData.Add(row);
			}

			return tableData;
		}

		public List<string[]> GenerateConverterTableData(List<ConversionInfo> converters)
		{
			var tableData = new List<string[]>();

			foreach (var converter in converters)
			{
				var row = new string[]
				{
					converter.Name,
					converter.InputResource,
					$"{converter.InputRate:F3}",
					converter.OutputResource,
					$"{converter.OutputRate:F3}",
					$"{converter.ECInput:F3}"
				};

				tableData.Add(row);
			}

			return tableData;
		}
	}
}