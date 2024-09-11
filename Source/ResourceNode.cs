using UnityEngine;

namespace KwarterMaster
{
    public class ResourceNode
    {
        private GUIStyle _titleStyle;
        private GUIStyle _inputStyle;
        private GUIStyle _inputDayStyle;
        private GUIStyle _ecUsageStyle;
        private GUIStyle _storageStyle;
        private GUIStyle _timeTillFullStyle;

        private static int _width = 300;
        public static int Width { get { return _width; } }
        private static int _height = 75;
        public static int Height { get { return _height; } }
        private static int _xSpacing = 100;
        public static int XSpacing { get { return _xSpacing; } }
        private static int _ySpacing = 10;
        public static int YSpacing { get { return _ySpacing; } }
        public string Name { get; private set; }
        public float AvailableStorage { get; private set; }
        // Levels for aligning the x position in the flow chart
        public int XLevel { get; set; }
        public int YLevel { get; set; }

        public float InputRate { get; set; }
        public float InputRateDay { get { return InputRate * 60 * 60 * 6; } }
        public float ECUsage { get; set; }
        public float TimeTillFull { get { return AvailableStorage / InputRateDay; } }

        public ResourceNode(string name)
        {
            Name = name;
            XLevel = -1;
            YLevel = -1;
            AvailableStorage = 0F;
            InputRate = 0F;
            ECUsage = 0F;
        }

        public void AddStorage(float amount)
        {
            AvailableStorage += amount;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(XLevel * (_width + _xSpacing), YLevel * (_height + _ySpacing));
        }

        private void GetStyles()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.UpperCenter, // Centered at the top
                    padding = new RectOffset(0, 0, 5, 0), // Padding only at the top
                    fontStyle = FontStyle.Bold,

                };
                _inputStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = null }, // No background
                    alignment = TextAnchor.UpperLeft,
                    padding = new RectOffset(10, 0, 5, 0),
                    fontSize = 14,
                };
                _inputDayStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = null }, // No background
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(10, 0, 0, 0),
                    fontSize = 14,
                };
                _ecUsageStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = null }, // No background
                    alignment = TextAnchor.LowerLeft,
                    padding = new RectOffset(10, 0, 0, 5),
                    fontSize = 14,
                };
                _storageStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = null }, // No background
                    alignment = TextAnchor.UpperRight,
                    padding = new RectOffset(0, 10, 5, 0),
                    fontSize = 14,
                };
                _timeTillFullStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = null }, // No background
                    alignment = TextAnchor.MiddleRight,
                    padding = new RectOffset(0, 10, 0, 5),
                    fontSize = 14,
                };
            }
        }

        public void Draw()
        {
            GetStyles();

            Vector2 position = GetPosition();
            Rect box = new Rect(position.x, position.y, _width, _height);

            // Draw the node title at the top
            GUI.Box(box, Name, _titleStyle);

            // Draw the input rate on the left
            GUI.Label(box, $"{InputRate:F2}/s", _inputStyle);
            GUI.Label(box, $"{InputRateDay / 1000:F2}k/d", _inputDayStyle);
            GUI.Label(box, $"{ECUsage:F2}EC/s", _ecUsageStyle);

            // Draw the available storage right
            GUI.Label(box, $"{AvailableStorage:F2}", _storageStyle);
            GUI.Label(box, $"{TimeTillFull:F2}d", _timeTillFullStyle);
        }
    }
}