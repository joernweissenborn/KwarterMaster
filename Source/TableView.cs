using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KwarterMaster
{
    public class TableView
    {
        private Vector2 _scrollPosition = Vector2.zero;
        private GUIStyle _headerStyle;
        private TableInfo _tableInfo;

        public TableView(TableInfo tableInfo)
        {
            _tableInfo = tableInfo;
        }

        private void InitializeHeaderStyle()
        {
            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };
        }

        // Public property to calculate the total width
        public int TotalWidth
        {
            get
            {
                return _tableInfo.ColumnSizes.Sum() + 20; // Adding some padding
            }
        }

        public void Draw()
        {
            if (_headerStyle == null)
            {
                InitializeHeaderStyle();
            }

            if (_tableInfo == null)
            {
                Debug.LogError("TableInfo is null in Draw method.");
                return;
            }

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(TotalWidth), GUILayout.Height(160));

            // Draw the header
            GUILayout.BeginHorizontal();
            if (_tableInfo.Header == null)
            {
                Debug.LogError("TableInfo.Header is null in Draw method.");
                return;
            }

            for (int i = 0; i < _tableInfo.Header.Length; i++)
            {
                GUILayout.Label(_tableInfo.Header[i], _headerStyle, GUILayout.Width(_tableInfo.ColumnSizes[i]));
            }
            GUILayout.EndHorizontal();

            // Draw the table data
            if (_tableInfo.Data == null)
            {
                Debug.LogError("TableInfo.Data is null in Draw method.");
                return;
            }

            foreach (var row in _tableInfo.Data)
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < row.Length; i++)
                {
                    GUILayout.Label(row[i], GUILayout.Width(_tableInfo.ColumnSizes[i]));
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        // Method to update the data
        public void SetData(List<string[]> newData)
        {
            if (newData == null)
            {
                Debug.LogError("New data is null in SetData method.");
                return;
            }
            _tableInfo.Data = newData;
        }
    }
}