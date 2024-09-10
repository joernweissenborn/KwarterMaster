using UnityEngine;

namespace KwarterMaster
{
    public class ResourceNode
    {
        private static int _width = 100;
        public static int Width { get { return _width; } }
        private static int _height = 100;
        public static int Height { get { return _height; } }
        public string Name { get; private set; }
        public float AvailableStorage { get; private set; }
        // Levels for aligning the x position in the flow chart
        public int XLevel { get; set; }
        public int YLevel { get; set; }

        public ResourceNode(string name, float availableStorage = 0f)
        {
            Name = name;
            AvailableStorage = availableStorage;
            XLevel = -1;
            YLevel = -1;
        }

        public void AddStorage(float amount)
        {
            AvailableStorage += amount;
        }

        // Draw the node at the given position
        public void Draw(Vector2 position)
        {
            GUI.Box(new Rect(position.x, position.y, _width, _height), Name);
        }
    }
}