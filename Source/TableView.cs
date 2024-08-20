using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KwarterMaster
{
    public class TableView
    {
        private Vector2 scrollPosition = Vector2.zero;

        // Sample table data
        private string[] _header = new string[] { "Drill Name", "Max Ore", "Max EC" };
        private int[] _columnSizes = new int[] { 200, 100, 100 };
        // Sample table data
        private List<string[]> _tableData = new List<string[]>()
        {
            new string[] { "Drill-O-Matic", "2.5", "15" },
            new string[] { "Drill-O-Matic", "2.5", "15" },
            new string[] { "Drill-O-Matic", "2.5", "15" },
            new string[] { "Drill-O-Matic Junior", "0.5", "3" },
            new string[] { "Drill-O-Matic Junior", "0.5", "3" },
        };
        private GUIStyle headerStyle;

        // Public property to calculate the total width
        public int TotalWidth
        {
            get
            {
                return _columnSizes.Sum() + 20; // Adding some padding
            }
        }

        public void Draw()
        {    // Ensure headerStyle is initialized
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(GUI.skin.label);
                headerStyle.fontStyle = FontStyle.Bold;
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(TotalWidth), GUILayout.Height(160));

            // Draw the header
            GUILayout.BeginHorizontal();
            for (int i = 0; i < _header.Length; i++)
            {
                GUILayout.Label(_header[i], headerStyle, GUILayout.Width(_columnSizes[i]));
            }
            GUILayout.EndHorizontal();

            // Draw the table data
            foreach (var row in _tableData)
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < row.Length; i++)
                {
                    GUILayout.Label(row[i], GUILayout.Width(_columnSizes[i]));
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        // Method to update the data (similar to QML's property bindings)
        public void SetData(List<string[]> newData)
        {
            _tableData = newData;
        }
    }
}