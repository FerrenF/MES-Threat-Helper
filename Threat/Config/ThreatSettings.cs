using MESHelper.Threat.Core;
using MESHelper.Threat.Profile;
using System.Xml.Serialization;

namespace MESHelper.Threat.Configuration
{

    [XmlRoot("ThreatSettings")]
    public class ThreatSettings : ThreatSettingsBase
    {
        [XmlArray("ThreatProfiles")]
        [XmlArrayItem("ConditionalThreatProfile")]
        public List<ConditionalThreatProfile> ThreatProfiles { get; set; }
            = new List<ConditionalThreatProfile>();

        public new ThreatSettings copy()
        {
            return new ThreatSettings
            {
                ThreatModVersion = this.ThreatModVersion,
                SingleBlockThreatEntries = this.SingleBlockThreatEntries,
                BlockCategoryThreatEntries = this.BlockCategoryThreatEntries,
                GridTypeMultipliers = this.GridTypeMultipliers,
                GridPowerOutputMultipliers = this.GridPowerOutputMultipliers,
                BoundingBoxSizeMultipliers = this.BoundingBoxSizeMultipliers,
                ThreatPerBlockMultipliers = this.ThreatPerBlockMultipliers,
                ThreatProfiles = this.ThreatProfiles
            };
        }
    }
}