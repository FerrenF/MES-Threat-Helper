using MESHelper.Threat.CategoryProvider;
using MESHelper.Threat.Profile;

namespace MESHelper.Threat.Util
{
    public static class ThreatUtil
    {
        public static string ExtractBlockType(string xsiType)
        {
            if (string.IsNullOrWhiteSpace(xsiType))
                return string.Empty;

            int underscoreIndex = xsiType.IndexOf('_');
            if (underscoreIndex >= 0 && underscoreIndex < xsiType.Length - 1)
            {
                return xsiType.Substring(underscoreIndex + 1);
            }

            return xsiType;
        }

        public static EntityThreatProfile ThreatProfileFromBlueprintData(BlueprintParser.BlueprintData blueprintData)
        {
            var config = new EntityThreatProfile
            {
                DisplayName = blueprintData.FirstGridDisplayName ?? string.Empty,
                GridType = blueprintData.FirstGridType ?? string.Empty             
            };

            foreach (var kvp in blueprintData.BlockCounts)
            {
                var tracker = new ProfileBlockTracker
                {
                    Type = kvp.Key.Type,
                    SubType = kvp.Key.Subtype,
                    Count = kvp.Value,
                    Category = new CategoryGuesstimator(MESHelperState.Instance.AppConfig.BlockDictionary).GetCategory($"{kvp.Key.Type}/{kvp.Key.Subtype}")
                    // Power, TotalCurrentVolume, TotalMaxVolume remain 0
                };

                config.Blocks.Add(tracker);
            }

            return config;
        }
    }
}
