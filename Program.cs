using System.Runtime.InteropServices;

namespace MESHelper
{
    internal static class Program
    {

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        [STAThread]
        static void Main()
        {
            MESHelperState.init_state();
            if (MESHelperState.Instance == null) return;

            MESHelperState.Instance.RegisterDocumentManager(new MESHelperDocumentManager(MESHelperState.Instance));
            MESHelperState.Instance.RegisterConfigManager(new MESHelperConfigvIEWManager(MESHelperState.Instance));
            MESHelperState.Instance.RegisterThreatProfileManager(new MESHelperThreatProfileManager(MESHelperState.Instance));

            var config = MESHelperAppConfig.Load();           
            MESHelperState.Instance.RegisterAppConfig(config);
            MESHelperState.Instance.TriggerAppConfigLoaded(config, MESHelperAppConfig.ConfigFilePath);
            ApplicationConfiguration.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);           
            

            if (MESHelperState.Instance.AppConfig.DebugConsole)
            {
                AllocConsole();
            }
            Application.Run(new MainView());
            MESHelperState.Instance.AppConfig.Save();
            if (MESHelperState.Instance.AppConfig.DebugConsole)
            {
                FreeConsole();
            }
        }
    }
}