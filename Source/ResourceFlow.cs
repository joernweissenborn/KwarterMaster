using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace KwarterMaster
{
    public class ResourceFlow
    {
        private Texture2D _lineTex;
        private static readonly float _lineWidth = 2.0f;
        private static GUIStyle _panelStyle;
        private static readonly float _panelWidth = 150;
        public ResourceNode Input { get; private set; }
        public ResourceNode Output { get; private set; }
        public float InputRate { get; set; }
        public float ActualInputRate { get; set; }
        public float Ratio { get { return Math.Min(ActualInputRate / InputRate, 1); } }
        public float OutputRate { get; set; }
        public float ActualOutputRate { get { return OutputRate * Ratio; } }
        public float ECUsage { get; set; }

        private void GetStyles()
        {
            if (_lineTex == null)
            {
                _lineTex = new Texture2D(1, 1);
            }
            Color color = Color.green;
            if (Ratio < 0.9)
            {
                color = Ratio < 0.5 ? Color.red : Color.yellow;
            }
            _lineTex.SetPixel(0, 0, color);
            _lineTex.Apply();

            if (_panelStyle == null)
            {
                _panelStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Normal,
                };
            }
        }

        public ResourceFlow(ResourceNode input, ResourceNode output, float inputRate, float outputRate, float ecUsage)
        {
            Input = input;
            Output = output;
            InputRate = inputRate;
            OutputRate = outputRate;
            ECUsage = ecUsage;
        }

        public ResourceFlow(ResourceNode output, float rate, float ecUsage)
        {
            Input = null;
            Output = output;
            OutputRate = rate;
            ECUsage = ecUsage;
        }
        public bool IsHarvester()
        {
            return Input == null;
        }

        public void Draw()
        {
            GetStyles();

            Vector2 from = Input.GetPosition();
            Vector2 to = Output.GetPosition();
            // Adjust positions to start and end in the middle of the right and left sides respectively
            Vector2 start = new Vector2(from.x + ResourceNode.Width, from.y + ResourceNode.Height / 2); // Right middle of the "from" node
            Vector2 end = new Vector2(to.x, to.y + ResourceNode.Height / 2); // Left middle of the "to" node


            // First, a horizontal line to the right of the "from" node
            Vector2 firstBend = new Vector2(end.x - _panelWidth - 25, start.y);

            // Then a vertical line to align with the "to" node's y position
            Vector2 secondBend = new Vector2(firstBend.x, end.y);

            // Finally, a horizontal line to the left to reach the "to" node
            Draw90DegreeBend(start, firstBend, secondBend, end);

            bool isOutputBelow = Input.YLevel < Output.YLevel;
            Rect panelRect = new Rect(firstBend.x + 5, (isOutputBelow ? secondBend : firstBend).y - 30, _panelWidth, 25);
            GUI.Box(panelRect, $"{ActualInputRate}({InputRate}) U/s", _panelStyle);

            Vector2 panelLowerLeft = new Vector2(panelRect.x, panelRect.y + panelRect.height);
            DrawStraightLine(isOutputBelow ? secondBend : firstBend, panelLowerLeft);

        }
        private void Draw90DegreeBend(Vector2 start, Vector2 firstBend, Vector2 secondBend, Vector2 end)
        {
            // Draw the segments to form the desired path
            DrawStraightLine(start, firstBend);
            DrawStraightLine(firstBend, secondBend);
            DrawStraightLine(secondBend, end);
        }

        private void DrawStraightLine(Vector2 start, Vector2 end)
        {
            if (_lineTex == null)
            {
                _lineTex = new Texture2D(1, 1);
            }
            float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * 180f / Mathf.PI;
            float length = Vector2.Distance(start, end);
            GUIUtility.RotateAroundPivot(angle, start);
            GUI.DrawTexture(new Rect(start.x, start.y, length, _lineWidth), _lineTex);
            GUI.matrix = Matrix4x4.identity; // Reset the matrix
        }
        public override string ToString()
        {
            return $"{Input?.Name ?? "Harvester"} -> {Output.Name} ({InputRate} ec: {ECUsage})";
        }


    }

}