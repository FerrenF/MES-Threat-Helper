using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MESHelper.Event
{
    public class DocumentLoadedEventArgs : EventArgs
    {
        public bool LoadedFromFile { get;}
        public object DocumentManager { get; }
        public string? FilePath { get; }
        public XDocument CurrentXmlDocument { get; }

        public DocumentLoadedEventArgs(
            object documentManager,
            string filePath,
            XDocument currentXmlDocument
            )
        {
            DocumentManager = documentManager;
            FilePath = filePath;
            CurrentXmlDocument = currentXmlDocument;
            LoadedFromFile = true;
        }
        public DocumentLoadedEventArgs(
          object documentManager,       
          XDocument currentXmlDocument
          )
        {
            DocumentManager = documentManager;
            CurrentXmlDocument = currentXmlDocument;
            LoadedFromFile = false;
        }
    }
}
