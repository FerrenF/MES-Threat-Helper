using MESHelper.Threat.Profile;
using MESHelper.Threat.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using static MESHelper.BlueprintParser;


namespace MESHelper
{


    public class MESHelperThreatProfileManager
    {
        public MESHelperState? CurrentState = MESHelperState.Instance;
        DataGridView? TargetGrid => State?.ViewThreatProfileViewInstance?.ThreatConfigProfileGrid;

        public BindingList<EntityThreatProfile> ViewBindingList;

        public event EventHandler<EntityThreatProfile>? ProfileAdded;
        public event EventHandler<EntityThreatProfile>? ProfileRemoved;
        public event EventHandler<EntityThreatProfile>? ProfileUpdated;

        public MESHelperState State;
        public MESHelperThreatProfileManager(MESHelperState state)
        {
            State = state;
            ViewBindingList = new BindingList<EntityThreatProfile>();
            State.OnAppConfigLoaded += State_OnAppConfigLoaded1;  
            State.OnProfileViewLoaded += State_OnProfileViewLoaded;
            State.OnChangedThreatConfigState += State_OnChangedThreatConfigState;
        }

        private void State_OnChangedThreatConfigState(object? sender, Event.ChangedConfigEventArgs e)
        {
            RefreshGrid();
        }

        private void State_OnAppConfigLoaded1(object? sender, Event.ConfigLoadedEventArgs e)
        {
            if (CurrentState == null) return;
            if (e.ConfigTypeLoaded != Event.ConfigLoadedEventArgs.ConfigType.AppConfig) return;
            RemoveAndLoadProfiles(CurrentState.AppConfig.ThreatProfiles);
        }

        private int State_OnProfileViewLoaded(object sender)
        {
            if (State.ThreatProfileManager == null || TargetGrid == null)
                throw new Exception($"{((State.ThreatProfileManager == null) ? "$State.ThreatProfileManager" : "TargetGrid")} was null during call to State_OnProfileViewLoaded " + System.Environment.StackTrace.ToString());

            if (TargetGrid.DataSource == null)
            {
                TargetGrid.DataSource = ViewBindingList;
            }


            TargetGrid.UserDeletingRow += TargetGrid_UserDeletingRow; ;
  
            State.ThreatProfileManager.ProfileAdded += (s, profile) =>
            {
               
                RefreshGrid();
            };
            State.ThreatProfileManager.ProfileRemoved += (s, profile) =>
            {         

                RefreshGrid();
            };
            State.ThreatProfileManager.ProfileUpdated += (s, profile) =>
            {
                var index = ViewBindingList.IndexOf(profile);
                if (index >= 0)
                {
                    ViewBindingList.ResetItem(index);
                   
                }
                RefreshGrid();
            };
            return 0;
        }

        private void TargetGrid_UserDeletedRow(object? sender, DataGridViewRowEventArgs e)
        {
            
        }

        private void TargetGrid_UserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
        {
            var config = e.Row.DataBoundItem as EntityThreatProfile;
            if (config != null)
            {
               CurrentState.AppConfig.RemoveProfile(config);
            }
        }

        private void TargetGrid_RowsRemoved(object? sender, DataGridViewRowsRemovedEventArgs e)
        {
           

         
        }

        private void RefreshGrid()
        {
            this.ViewBindingList.ResetBindings();
            if (TargetGrid != null) TargetGrid.Refresh();
        }

        public void AddProfile(EntityThreatProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));
            if (ViewBindingList.Contains(profile)) ViewBindingList.Remove(profile);
            ViewBindingList.Add(profile);
            CurrentState.AppConfig.AddProfile(profile);
            ProfileAdded?.Invoke(this, profile);
        }
        public bool RemoveProfile(EntityThreatProfile profile)
        {
            if (ViewBindingList.Remove(profile))
            {
                ProfileRemoved?.Invoke(this, profile);
                CurrentState.AppConfig.RemoveProfile(profile);
                return true;
            }
            return false;
        }
        public bool UpdateProfile(EntityThreatProfile oldProfile, EntityThreatProfile newProfile)
        {
            if (oldProfile == null || newProfile == null)
                throw new ArgumentNullException();

            var index = ViewBindingList.IndexOf(oldProfile);
            if (index >= 0){
                ViewBindingList[index] = newProfile;
                ProfileUpdated?.Invoke(this, newProfile);
                return true;
            }
            return false;
        }

        public EntityThreatProfile? FindByDisplayName(string displayName)
        {
            return ViewBindingList.FirstOrDefault(p =>
                p.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<EntityThreatProfile> FindByGridSize(string gridSize)
        {
            return ViewBindingList.Where(p =>
                p.GridType.Equals(gridSize, StringComparison.OrdinalIgnoreCase));
        }

        public void RemoveAndLoadProfiles(IEnumerable<EntityThreatProfile> profiles)
        {
            ViewBindingList.Clear();
            foreach (var p in profiles)
                AddProfile(p);

            if (State.AppConfig == null) return;
            if (ViewBindingList == null) return;
            State.AppConfig.SaveProfiles(ViewBindingList.ToList());
        }

        public void LoadProfilesFromXml(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Threat profiles file not found", filePath);

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<EntityThreatProfile>));
            using var reader = new StreamReader(filePath);
            var list = (List<EntityThreatProfile>)serializer.Deserialize(reader);
            State.AppConfig.SaveProfiles(list);

        }

        public static List<EntityThreatProfile> DeserializeFromString(string xmlContent)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<EntityThreatProfile>));
            using var reader = new StringReader(xmlContent);
            return (List<EntityThreatProfile>)serializer.Deserialize(reader);
        }

      

        public void ImportConfigurationFromFile(string file)
        {
            try
            {
                var blueprintData = BlueprintParser.ParseBlueprint(file);
                foreach(BlueprintData bp in blueprintData)
                {
                    var config = ThreatUtil.ThreatProfileFromBlueprintData(bp);
                    AddProfile(config);
                    Console.WriteLine($"Profile added: {config.DisplayName} ({config.GridType})");
                }                       
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error attempting to add profile from: {file}: {ex.Message}");
            }       
        }
    }
    }



