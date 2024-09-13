using UnityEngine;

namespace KwarterMaster
{
    public class ToolbarView
    {
        private EfficiencyManager efficiencyManager;

        public ToolbarView(EfficiencyManager manager)
        {
            efficiencyManager = manager;
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal();

            // Checkbox
            bool engineerPresent = GUILayout.Toggle(efficiencyManager.IsEngineerOnBoard, "Engineer Present");
            if (engineerPresent != efficiencyManager.IsEngineerOnBoard)
            {
                efficiencyManager.IsEngineerOnBoard = engineerPresent;
            }


            if (efficiencyManager.IsEngineerOnBoard)
            {
                GUILayout.Label("Engineer Stars");
                string engineerStarsStr = GUILayout.TextField(efficiencyManager.EngineerStars.ToString());
                if (int.TryParse(engineerStarsStr, out int engineerStars))
                {
                    efficiencyManager.EngineerStars = Mathf.Clamp(engineerStars, 0, 5);
                }
            }
            GUILayout.FlexibleSpace();
            // Display efficiency multiplier
            GUILayout.Label($"Efficiency Multiplier: {efficiencyManager.CalculateEfficiencyMultiplier():F2}");
            GUILayout.EndHorizontal();
        }
    }
}