using System;
using System.Security.Cryptography;
using UnityEngine;

namespace KwarterMaster
{
    public class ResourceFlow
    {
        private static Texture2D _lineTex;
        public ResourceNode Input { get; private set; }
        public ResourceNode Output { get; private set; }
        public float Rate { get; set; }
        public float ECUsage { get; set; }

        public ResourceFlow(ResourceNode input, ResourceNode output, float rate, float ecUsage)
        {
            Input = input;
            Output = output;
            Rate = rate;
            ECUsage = ecUsage;
        }

        public ResourceFlow(ResourceNode output, float rate, float ecUsage)
        {
            Input = null;
            Output = output;
            Rate = rate;
            ECUsage = ecUsage;
        }
        public bool IsHarvester()
        {
            return Input == null;
        }

        public void Draw()
        {
            if (Input == null)
            {
                return;
            }
            Vector2 from = Input.GetPosition();
            Vector2 to = Output.GetPosition();
            // Adjust positions to start and end in the middle of the right and left sides respectively
            Vector2 start = new Vector2(from.x + ResourceNode.Width, from.y + ResourceNode.Height/2); // Right middle of the "from" node
            Vector2 end = new Vector2(to.x, to.y + ResourceNode.Height/2); // Left middle of the "to" node

            // First, a horizontal line to the right of the "from" node
            Vector2 firstBend = start + new Vector2(50, 0);

            // Then a vertical line to align with the "to" node's y position
            Vector2 secondBend = new Vector2(firstBend.x, end.y);

            // Finally, a horizontal line to the left to reach the "to" node
            Draw90DegreeBend(start, firstBend, secondBend, end);
        }
       private static void Draw90DegreeBend(Vector2 start, Vector2 firstBend, Vector2 secondBend, Vector2 end, float width = 2.0f)
        {
            // Draw the segments to form the desired path
            DrawStraightLine(start, firstBend, width);
            DrawStraightLine(firstBend, secondBend, width);
            DrawStraightLine(secondBend, end, width);
        }

        private static void DrawStraightLine(Vector2 start, Vector2 end, float width = 2.0f)
        {
            if (_lineTex == null)
            {
                _lineTex = new Texture2D(1, 1);
            }
            float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * 180f / Mathf.PI;
            float length = Vector2.Distance(start, end);
            GUIUtility.RotateAroundPivot(angle, start);
            GUI.DrawTexture(new Rect(start.x, start.y, length, width), _lineTex);
            GUI.matrix = Matrix4x4.identity; // Reset the matrix
        }
        public override string ToString()
        {
            return $"{Input?.Name ?? "Harvester"} -> {Output.Name} ({Rate} ec: {ECUsage})";
        }


    }

}