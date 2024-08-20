using UnityEngine;
using KSP.UI.Screens;

namespace KwarterMaster
{
    public class MainWindow : MonoBehaviour
    {
        private Rect windowRect = new Rect(200, 200, 500, 200);
        private bool showWindow = true;

        private TableView tableView;
        private ButtonView closeButton;
        
        private ApplicationLauncherButton toolbarButton;
        private Texture2D buttonTexture;

        void Start()
        {
            tableView = new TableView();
            closeButton = new ButtonView("Close", () => showWindow = false);

            buttonTexture = new Texture2D(38, 38);

            if (ApplicationLauncher.Instance != null)
            {
                toolbarButton = ApplicationLauncher.Instance.AddModApplication(
                    OnToolbarButtonClick,
                    OnToolbarButtonClick,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                    buttonTexture
                );
            }
        }
    
    void OnDestroy()
    {
        // Unregister the toolbar button
        if (toolbarButton != null)
        {
            ApplicationLauncher.Instance.RemoveModApplication(toolbarButton);
        }
    }

    void OnToolbarButtonClick()
    {
        showWindow = !showWindow;
    }
        void OnGUI()
        {
            // Apply the stock KSP skin
            GUI.skin = HighLogic.Skin;

            if (showWindow)
            {
                windowRect.width = tableView.TotalWidth;
                windowRect = GUILayout.Window(0, windowRect, DrawWindow, "My Mod Window");
            }
        }

        void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();

            // Draw the table and close button using KSP's skin
            tableView.Draw();
            closeButton.Draw();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }
}
