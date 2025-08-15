using System.Xml.Linq;

namespace MESHelper
{
    public static class BlueprintParser
    {
        private static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

        public enum BlueprintType
        {
            Player = 0,
            Prefab = 1,
            Other = 2,
        }
        public class BlueprintData
        {
            public Dictionary<(string Type, string Subtype), int> BlockCounts { get; set; } = new();
            public string FirstGridDisplayName { get; set; }
            public string FirstGridType { get; set; }

            public BlueprintType Type { get; set; }
        }

        public static List<BlueprintData> ParseBlueprint(string blueprintFilePath)
        {
            var contained = new List<BlueprintData>();
            var doc = XDocument.Load(blueprintFilePath);


            var allBlueprints = GetBlueprintList(doc);
            foreach (var blueprint in allBlueprints)
            {
                var bpType = GetBlueprintType(doc);
                var cubeGrids = GetCubeGrids(doc);
                var data = new BlueprintData();
                foreach (var cubeGrid in cubeGrids)
                {
                    // Set first display name if not already set
                    if (string.IsNullOrEmpty(data.FirstGridDisplayName))
                    {
                        var name = GetGridDisplayName(cubeGrid);
                        if (!string.IsNullOrEmpty(name))
                            data.FirstGridDisplayName = name;
                    }

                    // Set first grid size if not already set
                    if (string.IsNullOrEmpty(data.FirstGridType))
                    {
                        var size = GetGridType(cubeGrid);
                        if (!string.IsNullOrEmpty(size))
                            data.FirstGridType = size;
                    }

                    // Count all blocks
                    foreach (var (type, subtype) in GetBlocksFromGrid(cubeGrid))
                    {
                        var key = (Type: type, Subtype: subtype);
                        if (data.BlockCounts.ContainsKey(key))
                            data.BlockCounts[key]++;
                        else
                            data.BlockCounts[key] = 1;
                    }
                }
                if(data != null) contained.Add(data);
            }           
            return contained;
        
        }

        private static IEnumerable<XElement> GetBlueprintList(XDocument doc)
        {
            var l1 = doc.Descendants("ShipBlueprint")?.ToList();
            if (l1 != null && l1.Count > 0)
                return l1;
            var l2 = doc.Descendants("Prefab")?.ToList();
            if(l2 != null && l2.Count > 0) 
                return l2;
            return new List<XElement>();
        }   

        private static object GetBlueprintType(XDocument doc)
        {
            if (doc.Descendants("ShipBlueprint").ToList().Count > 0) return BlueprintType.Player;
            if (doc.Descendants("Prefab").ToList().Count > 0) return BlueprintType.Prefab;
            return BlueprintType.Other;
        }

        // Helpers

        private static IEnumerable<XElement> GetCubeGrids(XDocument doc)
        {
            return doc.Descendants("CubeGrid");
        }

        private static string GetGridDisplayName(XElement cubeGrid)
        {
            return cubeGrid.Element("DisplayName")?.Value?.Trim() ?? string.Empty;
        }

        private static string GetGridType(XElement cubeGrid)
        {
            return cubeGrid.Element("IsStatic")?.Value != null ? "Static" :
             cubeGrid.Element("GridSizeEnum")?.Value?.Trim() ?? string.Empty;
        }

        private static IEnumerable<(string Type, string Subtype)> GetBlocksFromGrid(XElement cubeGrid)
        {
            var cubeBlocks = cubeGrid.Element("CubeBlocks");
            if (cubeBlocks == null)
                yield break;

            foreach (var block in cubeBlocks.Elements())
            {
                string fullType = block.Attribute(xsi + "type")?.Value ?? "";
                string type = ExtractBlockType(fullType);

                string subtype = block.Element("SubtypeName")?.Value?.Trim() ?? string.Empty;

                yield return (type, subtype);
            }
        }

        private static string ExtractBlockType(string xsiType)
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
    }
}
