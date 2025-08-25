using MESHelper.Event;
using MESHelper.Threat;
using MESHelper.Threat.Configuration;
using MESHelper.Threat.Util;
using System.ComponentModel;
using System.Xml.Linq;

namespace MESHelper
{
 
    
    public class MESHelperState : INotifyPropertyChanged  
    {

        public static TLogInterface logger = new TDesktopLogger();

        public event PropertyChangedEventHandler? PropertyChanged;
        public void ChangeProperty(string propertyName){
              PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public delegate void RequestDocumentToConfigSyncDelegate();
        public delegate void RequestConfigToDocumentSyncDelegate();
        public event RequestConfigToDocumentSyncDelegate OnRequestDocumentFromConfigSync;
        public event RequestDocumentToConfigSyncDelegate OnRequestConfigFromDocumentSync;
        public void RequestConfigToDocumentSync() => OnRequestDocumentFromConfigSync?.Invoke();
        public void RequestDocumentToConfigSync() => OnRequestConfigFromDocumentSync?.Invoke();



        public delegate int OnCurrentDocumentUnloadArgs(object sender);
        public delegate int OnCurrentDocumentLoadArgs(object sender, XDocument? newDocument);



        public delegate int OnCurrentDocumentChangedArgs(object sender, XDocument? previousDocument, XDocument? newDocument);
        public event OnCurrentDocumentChangedArgs OnCurrentDocumentChanged;



        public event EventHandler<ConfigLoadedEventArgs> OnAppConfigLoaded;
        public void TriggerAppConfigLoaded(object sender, string path) => OnAppConfigLoaded?.Invoke(this, new ConfigLoadedEventArgs(path, ConfigLoadedEventArgs.ConfigType.AppConfig));




        public event EventHandler<ChangedAppConfigEventArgs> OnAppConfigChanged;
        public void TriggerAppConfigChanged(object sender, MESHelperAppConfig appConfig, ChangedAppConfigEventArgs.AppConfigClangedPropertyFlags propertyChangeFlags) =>
            OnAppConfigChanged?.Invoke(this, new ChangedAppConfigEventArgs(this, appConfig, propertyChangeFlags));



        public event EventHandler<DocumentLoadedEventArgs> OnDocumentLoadedFromXML;
        public void TriggerDocumentLoadedFromXML(object sender, string filePath,XDocument currentXmlDocument) => OnDocumentLoadedFromXML?.Invoke(this, new DocumentLoadedEventArgs(sender, filePath, currentXmlDocument));



        public event EventHandler<ChangedConfigEventArgs> OnChangedThreatConfigState;

        public void TriggerChangedThreatConfigState(object sender, ThreatSettings currentThreatConfig, ThreatSettings? oldThreatConfig) 
            => OnChangedThreatConfigState?.Invoke(sender, new ChangedConfigEventArgs(sender, currentThreatConfig, oldThreatConfig));



        public delegate int OnFormsLoadedArgs(object sender);

        public event OnFormsLoadedArgs OnMainFormLoaded;
        public event OnFormsLoadedArgs OnProfileViewLoaded;
        public event OnFormsLoadedArgs OnBlockDictionaryViewLoaded;

        public void MainFormLoaded(object sender) => this.OnMainFormLoaded?.Invoke(sender);
        public void ProfileViewLoaded(object sender) => this.OnProfileViewLoaded?.Invoke(sender);
        public void BlockDictionaryViewLoaded(object sender) => this.OnBlockDictionaryViewLoaded?.Invoke(sender);

        public static MESHelperState? Instance = null;




        public static MESHelperThreatProfileManager? threatProfileManager;
        public MESHelperThreatProfileManager ThreatProfileManager
        {
            get => threatProfileManager ?? (threatProfileManager = new MESHelperThreatProfileManager(this) );
            set
            {
                threatProfileManager = value;
                ChangeProperty("ThreatProfileManager");
            }
        }
        public void RegisterThreatProfileManager(MESHelperThreatProfileManager manager) => ThreatProfileManager = manager;



        private static MESHelperConfigvIEWManager? configManager;
        public MESHelperConfigvIEWManager ConfigManager
        {
            get => configManager ?? (configManager = new MESHelperConfigvIEWManager(this));
            set
            {
                configManager = value;
                ChangeProperty("ConfigManager");
            }
        }
        public void RegisterConfigManager(MESHelperConfigvIEWManager manager) => ConfigManager = manager;


        private static MESHelperDocumentManager? documentManager;
        public MESHelperDocumentManager DocumentManager
        {
            get => documentManager ?? (documentManager = new MESHelperDocumentManager(this));
            set
            {
                documentManager = value;
                ChangeProperty("DocumentManager");
            }
        }
        public void RegisterDocumentManager(MESHelperDocumentManager manager) => DocumentManager = manager;




        private static MESHelperAppConfig? appConfig;
        public MESHelperAppConfig AppConfig
        {
            get => appConfig ?? (appConfig = new MESHelperAppConfig());
            set
            {
                appConfig = value;
                ChangeProperty("AppConfig");
            }
        }
        public void RegisterAppConfig(MESHelperAppConfig appConfig) => AppConfig = appConfig;



        public static bool StateInitialized { get => Instance != null; }
        public static MESHelperState? init_state() {
            if(StateInitialized) return Instance;
            Instance = new MESHelperState();
            ThreatEvaluator.Logger = TDesktopLogger.Default;
            return Instance;
        }




       

        public bool xmlFileLoaded { get; set; }

     
        public void TriggerChangedCurrentXMLDocument(object sender, XDocument? oldDocument, XDocument? newDocument) => OnCurrentDocumentChanged?.Invoke(sender, oldDocument, newDocument);
        //public void UnloadCurrentXmlDocument(object sender) => OnCurrentDocumentUnload?.Invoke(sender);
        //public void LoadCurrentXmlDocument(object sender, XDocument? newDocument) => OnCurrentDocumentLoad?.Invoke(sender, newDocument);

        private XDocument? _currentXmlDocument = null;
        public XDocument? CurrentXmlDocument
        { 
            get => _currentXmlDocument;
            set
            { 
                _currentXmlDocument = value;               
            }
        }


    

        private ThreatSettings _currentThreatConfiguration;
        public ThreatSettings CurrentThreatConfiguration
        {
            get => _currentThreatConfiguration;
            set
            {                 
                _currentThreatConfiguration = value;
            }
        }



        private MainView? _mainViewInstance = null;
        public MainView? MainViewInstance
        { 
            get => _mainViewInstance;
            set 
            {
                _mainViewInstance = value;
            }
        }



        private ViewThreatProfile? _threatProfileViewInstance = null;

        public ViewThreatProfile? ViewThreatProfileViewInstance
        {
            get => _threatProfileViewInstance;
            set
            {
                _threatProfileViewInstance = value;
            }
        }


        private ViewBlockDictionary? _viewBlockDictionaryInstance = null;
        public ViewBlockDictionary? ViewBlockDictionaryInstance
        {
            get => _viewBlockDictionaryInstance;
            set
            {
                _viewBlockDictionaryInstance = value;
            }
        }

        public void ShowBlockDictionaryView()
        {
            if (_viewBlockDictionaryInstance == null || _viewBlockDictionaryInstance.IsDisposed)
                _viewBlockDictionaryInstance = new ViewBlockDictionary();
            if (!_viewBlockDictionaryInstance.Visible)
                _viewBlockDictionaryInstance.Show();
            else
                _viewBlockDictionaryInstance.BringToFront();
        }

        public void ShowThreatProfileView()
        {
            if (_threatProfileViewInstance == null || _threatProfileViewInstance.IsDisposed)
                _threatProfileViewInstance = new ViewThreatProfile();
            if (!_threatProfileViewInstance.Visible)
                _threatProfileViewInstance.Show();
            else
                _threatProfileViewInstance.BringToFront();
        }

        
    }
}
