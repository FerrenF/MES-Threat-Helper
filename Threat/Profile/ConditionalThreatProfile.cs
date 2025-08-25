using MESHelper.Threat.Configuration;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MESHelper.Threat.Profile {

    [XmlRoot("ConditionalThreatProfile")]
    public class ConditionalThreatProfile : ThreatSettingsBase
    {
        public enum ThreatProfileIdentifierType
        {
            Planet,
            Sector,
            Grid,
            FactionName
        }
        public enum ThreatProfileConditionType
        {
            Near,
            Is,
            Contains
        }

        [JsonInclude]
        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonInclude]
        [XmlAttribute("Identifier")]
        public ThreatProfileIdentifierType Identifier;

        [JsonInclude]
        [XmlAttribute("Condition")]
        public ThreatProfileConditionType Condition;

        [JsonInclude]
        [XmlAttribute("Value")]
        public string Value;

        [JsonInclude]
        [XmlAttribute("Importance")]
        public int Importance;

        public new ConditionalThreatProfile copy()
        {
            return new ConditionalThreatProfile
            {
                ThreatModVersion = this.ThreatModVersion,
                SingleBlockThreatEntries = this.SingleBlockThreatEntries,
                BlockCategoryThreatEntries = this.BlockCategoryThreatEntries,
                GridTypeMultipliers = this.GridTypeMultipliers,
                GridPowerOutputMultipliers = this.GridPowerOutputMultipliers,
                BoundingBoxSizeMultipliers = this.BoundingBoxSizeMultipliers,
                ThreatPerBlockMultipliers = this.ThreatPerBlockMultipliers,
                Name = this.Name,
                Identifier = this.Identifier,
                Condition = this.Condition,
                Value = this.Value,
                Importance = this.Importance,
            };
        }
    }
}