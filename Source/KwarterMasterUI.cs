using UnityEngine;
using KwarterMaster;

namespace KwarterMaster
{
    [KSPAddon(KSPAddon.Startup.EditorAny , false)]
    public class KwarterMasterUI : MonoBehaviour
    {
        private MainWindow mainWindow;

        void Start()
        {
            // Initialize the main window
            mainWindow = gameObject.AddComponent<MainWindow>();
        }
    }
}