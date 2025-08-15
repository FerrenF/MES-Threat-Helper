using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MESHelper.Event
{
    public class ConfigLoadedEventArgs : EventArgs
    {
        public enum ConfigType
        {
            AppConfig
        }
        public bool LoadedFromFile { get; set; }
        public string FilePath { get; set; }
        public ConfigType ConfigTypeLoaded { get; }

        public ConfigLoadedEventArgs(            
            string filePath,
            ConfigType configTypeLoaded
            )
        {
            FilePath = filePath;
            ConfigTypeLoaded = ConfigTypeLoaded;
            LoadedFromFile = true;
        }
      
    }
}
