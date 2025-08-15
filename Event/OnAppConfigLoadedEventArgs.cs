using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESHelper.Event
{
    public class AppConfigLoadedEventArgs : EventArgs
    {
        public object Sender { get; }
        public MESHelperAppConfig? AppConfig { get; }
        
        public AppConfigLoadedEventArgs(
            object sender       ,
            MESHelperAppConfig config
            )
        {
            Sender = sender;
            AppConfig = config;
        }

    }
}
