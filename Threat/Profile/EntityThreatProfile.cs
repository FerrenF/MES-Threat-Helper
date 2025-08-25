using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MESHelper.Threat.Profile
{

    [XmlRoot("EntityThreatProfile")]
    public class EntityThreatProfile
    {
        [JsonInclude]
        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; }

        [JsonInclude]
        [XmlAttribute("GridType")]
        public string GridType { get; set; }

        [JsonInclude]
        [XmlAttribute("GridScale")]
        public float GridScale { get; set; }

        [JsonInclude]
        [XmlArray("Blocks")]
        [XmlArrayItem("ProfileBlockTracker")]
        public HashSet<ProfileBlockTracker> Blocks { get; set; } = new HashSet<ProfileBlockTracker>();

        [JsonIgnore]
        [XmlIgnore]
        public int NumBlocks => Blocks.Sum((b) => b.Count);

        [JsonIgnore]
        [XmlIgnore]
        public float Threat => new ThreatEvaluator(this).evaluate();
    }
}
