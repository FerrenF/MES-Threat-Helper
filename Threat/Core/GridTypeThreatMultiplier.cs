using System.Xml.Serialization;

namespace MESHelper.Threat.Core
{
    public class GridTypeThreatMultiplier
    {
        public float SmallGridMultiplier { get; set; } = 1.0f;
        public float LargeGridMultiplier { get; set; } = 1.0f;
        public float StationMultiplier { get; set; } = 1.0f;
    }
}