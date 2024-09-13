using UnityEngine;
using KSP.UI.Screens;

namespace KwarterMaster
{
    [KSPAddon(KSPAddon.Startup.EditorAny , false)]
    public class KwarterMasterUI : MonoBehaviour
    {
        private static readonly string _windowTitle = "KwaterMaster";
        private ResourceView _resourceView;
        private ToolbarView _toolbarView;
        private ButtonView _closeButton;
        private Texture2D _buttonTexture;
        private ApplicationLauncherButton _toolbarButton;
        private bool _showWindow;
        private Rect _windowRect;
        private EfficiencyManager _efficiencyManager;
        private HarvesterManager _harvesterManager;
        private ConversionManager _converterManager;
        private ResourceFlowGraph _resourceFlowGraph;
        void Start()
        {
            _windowRect = new Rect(100, 100, 1000, 300);

            _resourceFlowGraph = new ResourceFlowGraph();
            _resourceFlowGraph.AddHarvester("Ore", 0.1f, 10);
            _resourceFlowGraph.AddHarvester("Fox", 0.1f, 10);
            _resourceFlowGraph.AddFlow("Ore", "Oxidizer", 0.5f, 0.2f, 10);
            _resourceFlowGraph.AddFlow("Ore", "LiquidFuel", 0.5f, 0.2f, 10);
            _resourceFlowGraph.AddHarvester("Water", 5f, 10);
            _resourceFlowGraph.AddFlow("Water", "o2", 0.5f, 0.2f, 10);
            _resourceFlowGraph.AddFlow("Water", "h2", 0.5f, 0.2f, 10);
            _resourceFlowGraph.AddFlow("o2", "Oxidizer", 0.5f, 0.2f, 10);
            _resourceFlowGraph.AddFlow("Fox", "Oxidizer", 0.5f, 0.2f, 10);
            _resourceFlowGraph.AssignXLevels();
            _resourceFlowGraph.AssignYLevels();
            _resourceFlowGraph.SetStorage("Ore", 10000);
            _resourceFlowGraph.SetStorage("Oxidizer", 100);
            _resourceFlowGraph.SetStorage("LiquidFuel", 100);
            _resourceFlowGraph.SetStorage("Water", 100);
            _resourceFlowGraph.SetStorage("o2", 100);
            _resourceFlowGraph.SetStorage("h2", 10);
            _resourceFlowGraph.DebugGraph();

            _efficiencyManager = new EfficiencyManager(true, 5, 50);
            _harvesterManager = new HarvesterManager();
            _converterManager = new ConversionManager();

            _closeButton = new ButtonView(new Rect(_windowRect.width - 25, 5, 20, 20), "X", () => _showWindow = false, Color.red);
            _resourceView = new ResourceView(_resourceFlowGraph, _windowRect.width);
            _toolbarView = new ToolbarView(_efficiencyManager);

            // Load the button texture
            _buttonTexture = new Texture2D(38, 38);
            // Load your texture here, for example:
            // _buttonTexture.LoadImage(File.ReadAllBytes("path_to_your_texture.png"));
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

            // Initialize other variables
            _showWindow = true;
        }

        void OnDestroy()
        {
            Debug.Log("MainWindow OnDestroy method called.");

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

            _converterManager.Update();
            _harvesterManager.Update();
            _resourceFlowGraph.CalculateProductionRates();

            _resourceView.Draw();

            _toolbarView.Draw();

            GUI.DragWindow();
        }

        private void UpdateHarvesterData()
        {
        }
    }
}