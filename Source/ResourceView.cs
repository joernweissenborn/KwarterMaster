using UnityEngine;

namespace KwarterMaster
{
    public class ResourceView
    {
        private static Texture2D _lineTex;
        private Vector2 _scrollPosition = Vector2.zero;
        private int _width;

        public ResourceView(int width)
        {
            _width = width;
        }

        public void Draw()
        {
            _scrollPosition = GUILayout.BeginScrollView(
                _scrollPosition, GUILayout.Width(_width), GUILayout.Height(160)
            );

            // Set up basic node positions
            Vector2 orePosition = new Vector2(10, 10);
            Vector2 lfPosition = new Vector2(220, 70);
            Vector2 oxPosition = new Vector2(220, 130);


            // Determine the scrollable area size based on node positions
            float contentHeight = Mathf.Max(orePosition.y, lfPosition.y, oxPosition.y) + 100;
            float contentWidth = Mathf.Max(orePosition.x, lfPosition.x, oxPosition.x) + 200;
            // Create a larger area for content to avoid clipping
            GUILayout.BeginHorizontal(GUILayout.Width(contentWidth), GUILayout.Height(contentHeight));
            GUILayout.BeginVertical();



            // Draw Ore node
            DrawNode("Ore", orePosition);

            // Draw LiquidFuel node
            DrawNode("LiquidFuel", lfPosition);

            // Draw Oxidizer node
            DrawNode("Oxidizer", oxPosition);

            // Draw connections
            DrawConnection(orePosition, lfPosition);
            DrawConnection(orePosition, oxPosition);


            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

        }

        void DrawNode(string resourceName, Vector2 position)
        {
            GUI.Box(new Rect(position.x, position.y, 100, 50), resourceName);
        }

        void DrawConnection(Vector2 from, Vector2 to)
        {
            // Adjust positions to start and end in the middle of the right and left sides respectively
            Vector2 start = new Vector2(from.x + 100, from.y + 25); // Right middle of the "from" node
            Vector2 end = new Vector2(to.x, to.y + 25); // Left middle of the "to" node

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
    }
}