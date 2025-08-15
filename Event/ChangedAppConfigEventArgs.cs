using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESHelper.Event
{
    public class ChangedAppConfigEventArgs : EventArgs
    {
        [Flags]
        public enum AppConfigClangedPropertyFlags
        {
            ThreatProfiles = 1 << 0,
            LastVersion = 1 << 1,
            RecentFiles = 1 << 2,
            BlockDictionary = 1 << 3
        }
        public object Sender { get; }
        public MESHelperAppConfig? AppConfig { get; }
        public AppConfigClangedPropertyFlags PropertyChangedFlags { get; }
        public ChangedAppConfigEventArgs(
            object sender,
            MESHelperAppConfig appConfig,
            AppConfigClangedPropertyFlags propertyChangedFlags
            )
        {
            Sender = sender;
            AppConfig = appConfig;
            PropertyChangedFlags = propertyChangedFlags;
        }

    }
}
