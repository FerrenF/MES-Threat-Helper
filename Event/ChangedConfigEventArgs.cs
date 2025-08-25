using MESHelper.Threat.Configuration;
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
        public ThreatSettings? OldConfig { get; }
        public ThreatSettings CurrentConfig { get; }

        public ChangedConfigEventArgs(
            object sender,
            ThreatSettings newConfig,
            ThreatSettings? oldThreat
            )
        {
            Sender = sender;
            OldConfig = oldThreat;
            CurrentConfig = newConfig;
        }

    }
}
