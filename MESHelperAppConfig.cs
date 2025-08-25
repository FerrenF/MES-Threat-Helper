using MESHelper.Event;
using MESHelper.Threat.Profile;
using Microsoft.VisualBasic.Logging;
using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MESHelper
{

    public class MESHelperAppConfig
    {       
        public static readonly string ConfigFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public List<BlockDictionaryEntry> BlockDictionary { get; set; } = new();
        public List<EntityThreatProfile> ThreatProfiles { get; set; } = new();
        public string LastVersion { get; set; } = "0.0.0.0";
        public List<string> RecentFiles { get; set; } = new();
        public bool DebugConsole { get; set; } = true;

        [JsonIgnore]
        public MESHelperState? CurrentState => MESHelperState.Instance;

        public static MESHelperAppConfig Load()
        {
            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<MESHelperAppConfig>(json);
                    if (config != null)
                        return config;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read config.json: {ex.Message}");
                }
            }

            // If file doesn't exist or deserialization failed, return new
            var newConfig = new MESHelperAppConfig();
            newConfig.Save();
            return newConfig;
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save config.json: {ex.Message}");
            }
        }

        public void AddRecentFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            filePath = Path.GetFullPath(filePath);
            RecentFiles.RemoveAll(f => string.Equals(f, filePath, StringComparison.OrdinalIgnoreCase));
            RecentFiles.Insert(0, filePath);

            if (RecentFiles.Count > 10)
                RecentFiles = RecentFiles.Take(10).ToList();

            Save();
        }

        public void SaveProfiles(List<EntityThreatProfile> profileList)
        {
            ThreatProfiles = profileList.ToList();
            Save();
        }

        public void AddProfile(EntityThreatProfile profile)
        {
            if (!ThreatProfiles.Contains(profile))
            {
                ThreatProfiles.Add(profile);
                Save();
            }
        }

        public void RemoveProfile(EntityThreatProfile profile)
        {
            if (ThreatProfiles.Remove(profile))
                Save();
        }

        public void SaveBlockDictionary(List<BlockDictionaryEntry> blockList)
        {
            BlockDictionary = blockList.ToList();
            Save();
        }

        public void AddBlockDictionaryEntry(BlockDictionaryEntry entry)
        {
            if (!BlockDictionary.Any(e =>
                e.Type.Equals(entry.Type, StringComparison.OrdinalIgnoreCase) &&
                e.SubType.Equals(entry.SubType, StringComparison.OrdinalIgnoreCase)))
            {
                BlockDictionary.Add(entry);
                Save();
            }
        }

        public void RemoveBlockDictionaryEntry(BlockDictionaryEntry entry)
        {
            if (BlockDictionary.RemoveAll(e =>
                e.Type.Equals(entry.Type, StringComparison.OrdinalIgnoreCase) &&
                e.SubType.Equals(entry.SubType, StringComparison.OrdinalIgnoreCase)) > 0)
            {
                Save();
            }
        }
        public void ResetBlockDictionary(object sender = null)
        {
            UpdatedAppConfig(sender, CurrentState.AppConfig, ChangedAppConfigEventArgs.AppConfigClangedPropertyFlags.BlockDictionary);
            BlockDictionary.Clear();
            Save();  
        }


        public void UpdatedAppConfig(object sender, MESHelperAppConfig newConfig,
            ChangedAppConfigEventArgs.AppConfigClangedPropertyFlags changedPropertyFlags)
        {
            CurrentState?.TriggerAppConfigChanged(sender, newConfig, changedPropertyFlags);
        }
    }

}