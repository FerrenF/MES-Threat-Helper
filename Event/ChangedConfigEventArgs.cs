using MESHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MESHelper.Event
{
    public class ChangedConfigEventArgs : EventArgs
    {
        public object Sender { get; }
        public ConfigThreat? OldConfig { get; }
        public ConfigThreat CurrentConfig { get; }

        public ChangedConfigEventArgs(
            object sender,
            ConfigThreat newConfig,
            ConfigThreat? oldThreat
            )
        {
            Sender = sender;
            OldConfig = oldThreat;
            CurrentConfig = newConfig;
        }

    }
}
