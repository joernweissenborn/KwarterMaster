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
            GUILayout.BeginVertical();

            // Checkbox
            bool engineerPresent = GUILayout.Toggle(efficiencyManager.IsEngineerOnBoard, "Engineer Present");
            if (engineerPresent != efficiencyManager.IsEngineerOnBoard)
            {
                efficiencyManager.IsEngineerOnBoard = engineerPresent;
            }

            if (efficiencyManager.IsEngineerOnBoard)
            {
                // Horizontal radio buttons for Engineer Stars
                GUILayout.Label("Engineer Stars");
                GUILayout.BeginHorizontal();
                for (int i = 0; i <= 5; i++)
                {
                    bool isSelected = efficiencyManager.EngineerStars == i;
                    if (GUILayout.Toggle(isSelected, i.ToString()))
                    {
                        efficiencyManager.EngineerStars = i;
                    }
                    GUILayout.Space(10); // Add space between radio buttons
                }
                GUILayout.EndHorizontal();
            }
            // Display efficiency multiplier
            GUILayout.Label($"Efficiency Multiplier: {efficiencyManager.CalculateEfficiencyMultiplier():F2}");
            // Input field
            GUILayout.Label("Resource Concentration[%]");
            string resourceConcentrationStr = GUILayout.TextField(efficiencyManager.ResourceConcentration.ToString());
            if (int.TryParse(resourceConcentrationStr, out int resourceConcentration))
            {
                efficiencyManager.ResourceConcentration = Mathf.Clamp(resourceConcentration, 0, 100);
            }

            // Calculate and display ore output
            //float actualOreOutput = efficiencyManager.CalculateOreOutput();
            //GUILayout.Label($"Actual Ore Output: {actualOreOutput:F2}");

            GUILayout.EndVertical();
        }
    }
}