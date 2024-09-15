using System;
using UnityEngine;

namespace KwarterMaster
{
    public abstract class ResourceNode
    {
        private GUIStyle _titleStyle;
        private GUIStyle _productionStyle;
        private GUIStyle _dailyProductionStyle;
        private GUIStyle _storageStyle;
        private static int _offset = 10;
        private static int _width = 200;
        public static int Width { get { return _width; } }
        private static int _height = 75;
        public static int Height { get { return _height; } }
        private static int _xSpacing = 200;
        public static int XSpacing { get { return _xSpacing; } }
        private static int _ySpacing = 10;
        public static int YSpacing { get { return _ySpacing; } }
        public string Name { get; private set; }

        public int XLevel { get; set; }
        public int YLevel { get; set; }

        public float Storage { get; set; }
        public float ProductionRate { get; set; }
        public float ProductionRateDay { get { return GetActualProductionRate() * 60 * 60 * 6; } }
        public float TimeTillFull { get { return Storage / ProductionRateDay; } }
        public float ECUsage { get; set; }

        public ResourceNode(string name)
        {
            Name = name;
            XLevel = -1;
            YLevel = -1;
            Storage = 0F;
            ProductionRate = 0F;
            ECUsage = 0F;
        }

        public abstract float GetActualProductionRate();

        public Vector2 GetPosition()
        {
            return new Vector2(XLevel * (_width + _xSpacing), YLevel * (_height + _ySpacing)) + new Vector2(_offset, _offset);
        }

        protected Rect Box()
        {
            Vector2 position = GetPosition();
            return new Rect(position.x, position.y, _width, _height);
        }

        protected virtual void GetStyles()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.UpperLeft,
                    padding = new RectOffset(5, 0, 5, 0),
                    fontStyle = FontStyle.Bold,

                };
                _productionStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.UpperRight,
                    padding = new RectOffset(0, 5, 5, 0),
                    fontSize = 14,
                };
                _dailyProductionStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleRight,
                    padding = new RectOffset(0, 5, 0, 0),
                    fontSize = 14,
                };
                _storageStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.LowerCenter,
                    padding = new RectOffset(0, 0, 0, 5),
                    fontSize = 14,
                };
            }
        }

        public virtual void Draw()
        {
            GetStyles();

            Rect box = Box();

            GUI.Box(box, Name, _titleStyle);

            GUI.Label(box, $"{GetActualProductionRate():F3} ({ProductionRate:F2}) U/s", _productionStyle);

            string unit = "";
            float productionRateDay = ProductionRateDay;
            if (productionRateDay > 1000)
            {
                productionRateDay /= 1000;
                unit = "k";
            }
            GUI.Label(box, $"{productionRateDay:F2} {unit}U/d", _dailyProductionStyle);

            float storageAmount = Storage;
            unit = "";
            if (storageAmount > 1000)
            {
                storageAmount /= 1000;
                unit = "k";
            }
            GUI.Label(box, $"Storage: {storageAmount:F2} {unit}U ({TimeTillFull:F2} d)", _storageStyle);
        }
    }

    // A singleton to store ressource concentrations
    public class ResourceConcentration
    {
        private static ResourceConcentration _instance;
        public static ResourceConcentration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ResourceConcentration();
                }
                return _instance;
            }
        }

        private ResourceConcentration()
        {
            Concentrations = new System.Collections.Generic.Dictionary<string, float>();
        }

        public System.Collections.Generic.Dictionary<string, float> Concentrations { get; private set; }
    }

    public class HarvesterNode : ResourceNode
    {
        private static GUIStyle _concentrationStyle;
        private static GUIStyle _concentrationLabelStyle;
        private float _concentration
        {
            get
            {
                if (ResourceConcentration.Instance.Concentrations.ContainsKey(Name))
                {
                    return ResourceConcentration.Instance.Concentrations[Name];
                }
                return 5;
            }
            set
            {
                ResourceConcentration.Instance.Concentrations[Name] = value;
            }
        }

        public HarvesterNode(string name) : base(name)
        {
        }

        protected override void GetStyles()
        {
            base.GetStyles();
            if (_concentrationStyle == null)
            {
                _concentrationStyle = new GUIStyle(GUI.skin.textField)
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                };
                _concentrationLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    //normal = { background = null },
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(70, 0, 0, 0),
                    fontSize = 14,
                };
            }
        }

        public override float GetActualProductionRate()
        {
            return ProductionRate * _concentration / 100;
        }

        public override void Draw()
        {
            base.Draw();

            Rect box = Box();
            Rect inputBox = new Rect(box.x + 5, box.y + box.height / 2 - 7.5F, 60, 15);
            string concentrationStr = GUI.TextField(inputBox, $"{_concentration:F2}", _concentrationStyle);
            GUI.Label(box, "%", _concentrationLabelStyle);
            if (float.TryParse(concentrationStr, out float concentration))
            {
                _concentration = Mathf.Clamp(concentration, 0, 100);
            }
        }
    }

    public class ProductNode : ResourceNode
    {
        private GUIStyle _effiencyStyle;
        public float ActualProductionRate { get; set; }
        public float Ratio { get { return ActualProductionRate / ProductionRate; } }
        public ProductNode(string name) : base(name)
        {
            ActualProductionRate = 0F;
        }

        protected override void GetStyles()
        {
            base.GetStyles();
            if (_effiencyStyle == null)
            {
                _effiencyStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(5, 0, 0, 0),
                    fontSize = 14,
                };
            }
            _effiencyStyle.normal.textColor = Ratio < 0.9 ? (Ratio < 0.5 ? Color.red : Color.yellow) : Color.green;
        }

        public override float GetActualProductionRate()
        {
            return ActualProductionRate;
        }

        public override void Draw()
        {
            base.Draw();
            Rect box = Box();
            GUI.Label(box, $"{Ratio * 100:F2}%", _effiencyStyle);
        }
    }
}