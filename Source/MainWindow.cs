using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens; // Add this line

namespace KwarterMaster
{
    public class MainWindow : MonoBehaviour
    {
        private enum TabType
        {
            Drill,
            Converter,
            Resource
        }

        private TabType _currentTab = TabType.Drill;
        private ButtonView _drillTabButton;
        private ButtonView _converterTabButton;
        private ButtonView _flowTabButton;
        private TableInfo _tableInfoHarvester;
        private TableView _tableViewHarvester;
        private TableInfo _tableInfoConverter;
        private TableView _tableViewConverter;
        private ResourceView _resourceView;
        private ToolbarView _toolbarView;
        private ButtonView _closeButton;
        private Texture2D _buttonTexture;
        private ApplicationLauncherButton _toolbarButton;
        private bool _showWindow = true;
        private Rect _windowRect;
        private EfficiencyManager _efficiencyManager;
        private HarvesterManager _harvesterManager;
        private ConversionManager _converterManager;

        void Start()
        {
            Debug.Log("MainWindow Start method called.");

            _closeButton = new ButtonView("Close", () => _showWindow = false);
            _drillTabButton = new ButtonView("Drills", () => _currentTab = TabType.Drill);
            _converterTabButton = new ButtonView("Converters", () => _currentTab = TabType.Converter);
            _flowTabButton = new ButtonView("Ressources", () => _currentTab = TabType.Resource);

            _harvesterManager = new HarvesterManager();
            _tableInfoHarvester = new TableInfo(new string[]
            { "Drill Name", "Output[Cur/Max]", "EC[Cur/Max]" }, new int[] { 400, 150, 150 });
            _tableViewHarvester = new TableView(_tableInfoHarvester);

            _converterManager = new ConversionManager();
            _tableInfoConverter = new TableInfo(new string[]
            { "Converter Name", "Input Resource", "Input Rate", "Output Resource", "Output Rate", "EC Input" },
                new int[] { 200, 100, 100, 100, 100, 100 });
            _tableViewConverter = new TableView(_tableInfoConverter);

            _efficiencyManager = new EfficiencyManager(true, 5, 50); // Example initialization
            _toolbarView = new ToolbarView(_efficiencyManager);

            _resourceView = new ResourceView(_tableViewHarvester.TotalWidth);

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
            _showWindow = false;
            _windowRect = new Rect(100, 100, 400, 300);
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
            Debug.Log("Toolbar button clicked.");
            _showWindow = !_showWindow;
        }

        void OnGUI()
        {
            //Debug.Log("MainWindow OnGUI method called.");

            // Apply the stock KSP skin
            GUI.skin = HighLogic.Skin;

            if (_showWindow)
            {
                if (_tableViewHarvester != null)
                {
                    _windowRect.width = _tableViewHarvester.TotalWidth;
                    _windowRect = GUILayout.Window(0, _windowRect, DrawWindow, "My Mod Window");
                }
                else
                {
                    Debug.LogError("_tableView is null in OnGUI method.");
                }
            }
        }

        private void DrawWindow(int windowID)
        {
            // Update harvester data
            UpdateHarvesterData();

            GUILayout.BeginHorizontal();
            _drillTabButton.Draw();
            _converterTabButton.Draw();
            _flowTabButton.Draw();

            GUILayout.EndHorizontal();

            switch (_currentTab)
            {
                case TabType.Drill:
                    _tableViewHarvester.Draw();
                    break;
                case TabType.Converter:
                    _tableViewConverter.Draw();
                    break;
                case TabType.Resource:
                    _resourceView.Draw();
                    break;
            }

            // Draw the toolbar
            _toolbarView.Draw();


            // Draw the close button
            _closeButton.Draw();

            GUI.DragWindow();
        }

        private void UpdateHarvesterData()
        {
            _converterManager.Update();
            _harvesterManager.Update();
            _tableInfoHarvester.Data = _efficiencyManager.GenerateHarvesterTableData(_harvesterManager.GetHarvesters());
            _tableInfoConverter.Data = _efficiencyManager.GenerateConverterTableData(_converterManager.GetConverters());
        }
    }
}