using UnityEngine;

namespace KwarterMaster
{
    public class ToolbarView
    {
        private readonly EfficiencyManager efficiencyManager;
        public float ECUsage { get; set; }

        public ToolbarView(EfficiencyManager manager)
        {
            efficiencyManager = manager;
            ECUsage = 0;
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Engineer On Board:");
            int engineerStars = GUILayout.SelectionGrid(
                efficiencyManager.IsEngineerOnBoard ? efficiencyManager.EngineerStars + 1 : 0,
                new string[] { "None", "L0", "L1", "L2", "L3", "L4", "L5" }, 7
            );
            efficiencyManager.EngineerStars = engineerStars - 1;
            efficiencyManager.IsEngineerOnBoard = engineerStars > 0;

            GUILayout.FlexibleSpace();
            // Display efficiency multiplier
            GUILayout.Label($"Base Efficiency: {efficiencyManager.Multiplier:P0}");
            GUILayout.Label($"EC Usage: {ECUsage:F2}");
            GUILayout.EndHorizontal();
        }
    }
}