using UnityEngine;
using UnityEngine.UI;

namespace KwarterMaster
{
    public class ButtonView
    {
        private readonly string _buttonText;
        private GUIStyle _style;
        private Color _color = Color.white;
        private readonly System.Action _onClick;
        private Rect _box;
        public Rect Box { get => _box; set => _box = value; }

        public ButtonView(string text, System.Action onClickAction)
        {
            _buttonText = text;
            _onClick = onClickAction;
        }
        public ButtonView(Rect box, string text, System.Action onClickAction)
        {
            _buttonText = text;
            _onClick = onClickAction;
            _box = box;
        }
        public ButtonView(Rect box, string text, System.Action onClickAction, Color color)
        {
            _buttonText = text;
            _onClick = onClickAction;
            _box = box;
            _color = color;
        }

        private void GetStyles()
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.button)
                {
                    normal = { textColor = _color },
                    alignment = TextAnchor.MiddleCenter
                };
            }
        }

        public void Draw()
        {
            GetStyles();
            bool button = _box != null ? GUI.Button(_box, _buttonText, _style) : GUILayout.Button(_buttonText, _style);
            if (button)
            {
                _onClick?.Invoke();
            }
        }
    }
}