using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MESHelper.Block
{
    [XmlRoot("BlockDictionaryEntry")]
    public class BlockDictionaryEntry 

    {
        public enum BlockSize
        {
            Small,
            Large
        }
        [XmlAttribute("Type")]
        public string Type { get; set; } = string.Empty;


        [XmlAttribute("SubType")]
        public string SubType { get; set; } = string.Empty;


        [XmlAttribute("Category")]
        public string Category { get; set; } = string.Empty;


        [XmlAttribute("MaxPowerOut")]
        public long MaxPower { get; set; } = 0;


        [XmlAttribute("MaxVolume")]
        public long MaxVolume { get; set; } = 0;


        [XmlAttribute("Size")]
        public BlockSize Size { get; set; } = BlockSize.Large;

    }
}
