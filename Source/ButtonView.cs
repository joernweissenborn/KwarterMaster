using UnityEngine;

namespace KwarterMaster
{
    public class ButtonView
    {
        private string buttonText;
        private System.Action onClick;

        public ButtonView(string text, System.Action onClickAction)
        {
            buttonText = text;
            onClick = onClickAction;
        }

        public void Draw()
        {
            if (GUILayout.Button(buttonText))
            {
                onClick?.Invoke();
            }
        }
    }
}