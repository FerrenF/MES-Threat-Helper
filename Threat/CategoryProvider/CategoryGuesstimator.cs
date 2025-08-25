using System.Xml.Linq;

namespace MESHelper.Threat.CategoryProvider
{
    public class CategoryGuesstimator : BlockCategoryProvider
    {
        
        List<BlockDictionaryEntry> entries;
        public CategoryGuesstimator(List<BlockDictionaryEntry> dictionary)
        {
            this.entries = dictionary;
        }
        public string Name { get => "Guesstimator"; }

        public string GetCategory(object obj)
        {
            if (obj is null) return string.Empty;
            if (obj is XElement)
            {
                return TryExtractCategory(obj as XElement);
            }
            else if (obj is string)
            {
                return ReallyDumbTryExtractCategory(obj as string);
            }
            else
                return string.Empty;
        }


        private string TryExtractCategory(XElement def)
        {

            string typeId = def.Element("Id")?.Element("TypeId")?.Value?.ToLower() ?? "";
            string subtypeId = def.Element("Id")?.Element("SubtypeId")?.Value?.ToLower() ?? "";
            string displayName = def.Element("DisplayName")?.Value?.ToLower() ?? "";

            if (typeId.Contains("drill") || subtypeId.Contains("drill") || displayName.Contains("drill"))
                return "Tools";
            if (typeId.Contains("grinder") || subtypeId.Contains("grinder") || displayName.Contains("grinder"))
                return "Tools";
            if (typeId.Contains("welder") || subtypeId.Contains("welder") || displayName.Contains("welder"))
                return "Tools";

            var resourceSinkGroup = def.Element("ResourceSinkGroup")?.Value;
            if (!string.IsNullOrEmpty(resourceSinkGroup))
            {
                switch (resourceSinkGroup)
                {
                    case "Factory":
                        return "Production";
                    case "Defense":
                        return "Turrets";
                    case "Thrust":
                        return "Thrusters";
                    case "BatteryBlock":
                        return "Power";
                    case "LifeSupport":
                        return "Medical";
                    case "Reactors":
                        return "Power";
                }
            }

            var powerOutput = def.Element("MaxPowerOutput")?.Value;
            if (!string.IsNullOrEmpty(powerOutput))
                return "Power";

            var xsiType = def.Attribute(XName.Get("type", "http://www.w3.org/2001/XMLSchema-instance"))?.Value;
            if (!string.IsNullOrEmpty(xsiType) && xsiType.Contains("CockpitDefinition"))
                return "Seats";
            if (typeId.Contains("inhibitor") || subtypeId.Contains("inhibitor") || displayName.Contains("inhibitor"))
                return "Inhibitors";
            if (typeId.Contains("cockpit") || subtypeId.Contains("cockpit") || displayName.Contains("cockpit"))
                return "Seats";
            if (typeId.Contains("parachute") || subtypeId.Contains("parachute") || displayName.Contains("parachute"))
                return "Parachutes";
            if (typeId.Contains("gyrosc") || subtypeId.Contains("gyrosc") || displayName.Contains("gyrosc"))
                return "Gyros";
            if (typeId.Contains("contract") || subtypeId.Contains("contract") || displayName.Contains("contract"))
                return "Contracts";
            if (typeId.Contains("medical") || subtypeId.Contains("medical") || displayName.Contains("medical"))
                return "Medical";
            if (typeId.Contains("detector") || subtypeId.Contains("detector") || displayName.Contains("detector"))
                return "Antennas";
            if (typeId.Contains("medical") || subtypeId.Contains("medical") || displayName.Contains("medical"))
                return "Medical";
            if (typeId.Contains("jumpdrive") || subtypeId.Contains("jumpdrive") || displayName.Contains("jumpdrive"))
                return "JumpDrives";
            if (typeId.Contains("gravity") || subtypeId.Contains("gravity") || displayName.Contains("gravity"))
                return "Gravity";
            if (typeId.Contains("virtualmass") || subtypeId.Contains("virtualmass") || displayName.Contains("virtualmass"))
                return "Gravity";
            if (typeId.Contains("spaceball") || subtypeId.Contains("spaceball") || displayName.Contains("spaceball"))
                return "Gravity";
            if (typeId.Contains("turret") || subtypeId.Contains("turret") || displayName.Contains("turret"))
                return "Turrets";
            if (typeId.Contains("thrust") || subtypeId.Contains("thrust") || displayName.Contains("thrust"))
                return "Thrusters";
            if (typeId.Contains("beacon") || subtypeId.Contains("beacon") || displayName.Contains("beacon"))
                return "Beacons";
            if (typeId.Contains("antenna") || subtypeId.Contains("antenna") || displayName.Contains("antenna"))
                return "Antennas";
            if (typeId.Contains("control") || subtypeId.Contains("control") || displayName.Contains("control"))
                return "Controllers";
            if (typeId.Contains("cargo") || subtypeId.Contains("cargo") || displayName.Contains("cargo"))
                return "Containers";
            if (typeId.Contains("piston") || subtypeId.Contains("piston") || displayName.Contains("piston"))
                return "Mechanical";
            if (typeId.Contains("merge") || subtypeId.Contains("merge") || displayName.Contains("merge"))
                return "Mechanical";
            if (typeId.Contains("rotor") || subtypeId.Contains("rotor") || displayName.Contains("rotor"))
                return "Mechanical";
            if (typeId.Contains("button") || subtypeId.Contains("button") || displayName.Contains("button"))
                return "Buttons";
            if (typeId.Contains("control") || subtypeId.Contains("control") || displayName.Contains("control"))
                return "Controllers";
            if (typeId.Contains("hinge") || subtypeId.Contains("hinge") || displayName.Contains("hinge"))
                return "Mechanical";
            if (typeId.Contains("turret") || subtypeId.Contains("turret") || displayName.Contains("turret"))
                return "Turret";
            if (typeId.Contains("missile") || subtypeId.Contains("missile") || displayName.Contains("missile"))
                return "Guns";
            if (typeId.Contains("nanobot") || subtypeId.Contains("nanobot") || displayName.Contains("nanobot"))
                return "NanoBots";
            if (typeId.Contains("shield") || subtypeId.Contains("shield") || displayName.Contains("shield"))
                return "Shield";
            if (typeId.Contains("survivalkit") || subtypeId.Contains("survivalkit") || displayName.Contains("survivalkit"))
                return "Medical";
            if (typeId.Contains("tank") || subtypeId.Contains("tank") || displayName.Contains("tank"))
                return "Production";
            if (typeId.Contains("generator") || subtypeId.Contains("generator") || displayName.Contains("generator"))
                return "Production";
            

            var targetingGroups = def.Element("TargetingGroups")?.Elements("string").Select(s => s.Value).ToList() ?? new List<string>();
            if (targetingGroups.Contains("Propulsion"))
                return "Thrusters";
            if (targetingGroups.Contains("PowerSystems"))
                return "Power";
            if (targetingGroups.Contains("Weapons"))
                return "Guns";

            if (!string.IsNullOrEmpty(resourceSinkGroup))
            {
                switch (resourceSinkGroup)
                {
                    case "Factory":
                        return "Production";
                    case "Defense":
                        return "Turrets";
                    case "Thrust":
                        return "Thrusters";
                    case "BatteryBlock":
                        return "Power";
                }
            }

            return String.Empty;
        }

        private string ReallyDumbTryExtractCategory(string objId)
        {
            string[] parsedId = objId.Split('/');
            if (parsedId.Length != 2) return String.Empty;

            string typeId = parsedId[0];
            string subtypeId = parsedId[1];

            var try1 = entries.FirstOrDefault((entry) => entry.Type == typeId && entry.SubType == subtypeId, null);
            if (try1 != null)
            {
                return try1.Category;
            }
            if (typeId.Contains("drill") || subtypeId.Contains("drill"))
                return "Tools";
            if (typeId.Contains("grinder") || subtypeId.Contains("grinder"))
                return "Tools";
            if (typeId.Contains("welder") || subtypeId.Contains("welder"))
                return "Tools";

            if (typeId.Contains("inhibitor") || subtypeId.Contains("inhibitor") )
                return "Inhibitors";
            if (typeId.Contains("cockpit") || subtypeId.Contains("cockpit"))
                return "Seats";
            if (typeId.Contains("parachute") || subtypeId.Contains("parachute") )
                return "Parachutes";
            if (typeId.Contains("gyrosc") || subtypeId.Contains("gyrosc"))
                return "Gyros";
            if (typeId.Contains("contract") || subtypeId.Contains("contract"))
                return "Contracts";
            if (typeId.Contains("medical") || subtypeId.Contains("medical") )
                return "Medical";
            if (typeId.Contains("detector") || subtypeId.Contains("detector") )
                return "Antennas";
            if (typeId.Contains("medical") || subtypeId.Contains("medical") )
                return "Medical";
            if (typeId.Contains("jumpdrive") || subtypeId.Contains("jumpdrive") )
                return "JumpDrives";
            if (typeId.Contains("gravity") || subtypeId.Contains("gravity") )
                return "Gravity";
            if (typeId.Contains("virtualmass") || subtypeId.Contains("virtualmass"))
                return "Gravity";
            if (typeId.Contains("spaceball") || subtypeId.Contains("spaceball") )
                return "Gravity";
            if (typeId.Contains("turret") || subtypeId.Contains("turret") )
                return "Turrets";
            if (typeId.Contains("thrust") || subtypeId.Contains("thrust") )
                return "Thrusters";
            if (typeId.Contains("beacon") || subtypeId.Contains("beacon") )
                return "Beacons";
            if (typeId.Contains("antenna") || subtypeId.Contains("antenna") )
                return "Antennas";
            if (typeId.Contains("control") || subtypeId.Contains("control") )
                return "Controllers";
            if (typeId.Contains("cargo") || subtypeId.Contains("cargo") )
                return "Containers";
            if (typeId.Contains("piston") || subtypeId.Contains("piston") )
                return "Mechanical";
            if (typeId.Contains("merge") || subtypeId.Contains("merge") )
                return "Mechanical";
            if (typeId.Contains("rotor") || subtypeId.Contains("rotor") )
                return "Mechanical";
            if (typeId.Contains("button") || subtypeId.Contains("button") )
                return "Buttons";
            if (typeId.Contains("control") || subtypeId.Contains("control") )
                return "Controllers";
            if (typeId.Contains("hinge") || subtypeId.Contains("hinge") )
                return "Mechanical";
            if (typeId.Contains("turret") || subtypeId.Contains("turret") )
                return "Turret";
            if (typeId.Contains("missile") || subtypeId.Contains("missile") )
                return "Guns";
            if (typeId.Contains("nanobot") || subtypeId.Contains("nanobot") )
                return "NanoBots";
            if (typeId.Contains("shield") || subtypeId.Contains("shield") )
                return "Shield";
            if (typeId.Contains("survivalkit") || subtypeId.Contains("survivalkit") )
                return "Medical";
            if (typeId.Contains("tank") || subtypeId.Contains("tank") )
                return "Production";
            if (typeId.Contains("vent") || subtypeId.Contains("vent"))
                return "Utility";
            return String.Empty;
        
    }
    }
}
