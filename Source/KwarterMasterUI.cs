using UnityEngine;
using KSP.UI.Screens;
using System.Runtime.Serialization.Formatters;

namespace KwarterMaster
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class KwarterMasterUI : MonoBehaviour
    {
        private static readonly string _windowTitle = "KwaterMaster";
        private ResourceView _resourceView;
        private ToolbarView _toolbarView;
        private ButtonView _closeButton;
        private Rect _closeButtonRect => new Rect(_windowRect.width - 25, 5, 20, 20);
        private Texture2D _buttonTexture;
        private ApplicationLauncherButton _toolbarButton;
        private bool _showWindow;
        private Rect _windowRect;
        private Vector2 _minWindowSize = new Vector2(500, 100);
        private bool _isResizing = false;
        private EfficiencyManager _efficiencyManager;
        private ResourceFlowGraph _resourceFlowGraph;
        private ResourcePartManager _resourcePartManager;
        void Start()
        {
            _windowRect = new Rect(400, 100, 700, 300);

            _resourceFlowGraph = new ResourceFlowGraph();
            _efficiencyManager = new EfficiencyManager();
            _resourcePartManager = new ResourcePartManager(_efficiencyManager, _resourceFlowGraph);
            //_resourceFlowGraph.AddHarvester("Ore", 0.1f, 10);
            //_resourceFlowGraph.AddHarvester("Fox", 0.1f, 10);
            //_resourceFlowGraph.AddFlow("Ore", "Oxidizer", 0.5f, 0.2f, 10);
            //_resourceFlowGraph.AddFlow("Ore", "LiquidFuel", 0.5f, 0.2f, 10);
            //_resourceFlowGraph.AddHarvester("Water", 5f, 10);
            //_resourceFlowGraph.AddFlow("Water", "o2", 0.5f, 0.2f, 10);
            //_resourceFlowGraph.AddFlow("Water", "h2", 0.5f, 0.2f, 10);
            //_resourceFlowGraph.AddFlow("o2", "Oxidizer", 0.5f, 0.2f, 10);
            //_resourceFlowGraph.AddFlow("Fox", "Oxidizer", 0.5f, 0.2f, 10);
            //_resourceFlowGraph.AssignXLevels();
            //_resourceFlowGraph.AssignYLevels();
            //_resourceFlowGraph.AddStorage("Ore", 10000);
            //_resourceFlowGraph.AddStorage("Oxidizer", 100);
            //_resourceFlowGraph.AddStorage("LiquidFuel", 100);
            //_resourceFlowGraph.AddStorage("Water", 100);
            //_resourceFlowGraph.AddStorage("o2", 100);
            //_resourceFlowGraph.AddStorage("h2", 10);
            //_resourceFlowGraph.DebugGraph();

            _closeButton = new ButtonView(_closeButtonRect, "X", () => _showWindow = false, Color.red);
            _resourceView = new ResourceView(_resourceFlowGraph);
            _toolbarView = new ToolbarView(_efficiencyManager);

            // Load the button texture
            //_buttonTexture = new Texture2D(38, 38);
            _buttonTexture = GameDatabase.Instance.GetTexture("KwarterMaster/Textures/KwarterMaster", false);
            // Register the toolbar button
            if (ApplicationLauncher.Instance != null)
            {
                _toolbarButton = ApplicationLauncher.Instance.AddModApplication(
                    OnToolbarButtonClick,
                    OnToolbarButtonClick,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                    _buttonTexture
                );

                if (_toolbarButton == null)
                {
                    Debug.LogError("Failed to create toolbar button.");
                }
            }
            else
            {
                Debug.LogError("ApplicationLauncher.Instance is null.");
            }
        }

        void OnDestroy()
        {

            // Unregister the toolbar button
            if (_toolbarButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(_toolbarButton);
            }
        }

        void OnToolbarButtonClick()
        {
            _showWindow = !_showWindow;
        }

        void OnGUI()
        {
            GUI.skin = HighLogic.Skin;

            if (_showWindow)
            {
                _windowRect = GUILayout.Window(0, _windowRect, DrawWindow, _windowTitle);
            }
        }

        private void DrawWindow(int windowID)
        {
            _closeButton.Draw();

            //Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            _resourceFlowGraph.Clear();
            //_resourceFlowGraph.DebugGraph();
            //Debug.Log("=====================================");
            _resourcePartManager.GetParts();
            //_resourceFlowGraph.DebugGraph();
            //Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            _resourceFlowGraph.Update();
            //_resourceFlowGraph.DebugGraph();
            _toolbarView.ECUsage = _resourceFlowGraph.TotalECUsage();

            _resourceView.Draw();

            _toolbarView.Draw();

            ResizeWindow();
            if (!_isResizing) GUI.DragWindow();
        }

        private void ResizeWindow()
        {
            // Resize handle (bottom-right corner)
            Rect resizeRect = new Rect(_windowRect.width - 16, _windowRect.height - 16, 16, 16);

            GUI.DrawTexture(resizeRect, Texture2D.whiteTexture); // Optionally, add a visual resize handle

            if (Event.current.type == EventType.MouseDown && resizeRect.Contains(Event.current.mousePosition))
            {
                _isResizing = true;  // Enable resizing
                Event.current.Use();
            }

            if (_isResizing)
            {
                _windowRect.width = Mathf.Max(_minWindowSize.x, Event.current.mousePosition.x + 5);
                _windowRect.height = Mathf.Max(_minWindowSize.y, Event.current.mousePosition.y + 5);
                _closeButton.Box = _closeButtonRect;

                if (Event.current.type == EventType.MouseUp || Event.current.rawType == EventType.MouseUp)
                {
                    _isResizing = false;  // Stop resizing when the mouse button is released
                    Event.current.Use();
                }
            }
        }

    }
}