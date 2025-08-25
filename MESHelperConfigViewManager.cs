using MESHelper.Threat.Configuration;
using MESHelper.Threat.Core;
using ScintillaNET;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MESHelper
{
    public class MESHelperConfigvIEWManager
    {
        private MESHelperState _state;
        private ThreatSettings CurrentThreatConfig => _state.CurrentThreatConfiguration;
        private Scintilla CurrentXMLEditor => _state.MainViewInstance.xmlEditor;

        private BindingList<BlockCategoryThreat> categoryBinding = new BindingList<BlockCategoryThreat>();
        private BindingList<SingleBlockThreat> blockBinding = new BindingList<SingleBlockThreat>();
        private DataGridView CategoryTable => _state.MainViewInstance.CategoryTable;
        private DataGridView BlockTable => _state.MainViewInstance.BlockTable;
        private DataGridView MultipliersTable => _state.MainViewInstance.GridTypeTable;


        public class NamedMultiplier
        {
            public string Name { get; set; } = "";
            public float SmallGridMultiplier { get; set; }
            public float LargeGridMultiplier { get; set; }
            public float StationMultiplier { get; set; }

            public NamedMultiplier() { }

            public NamedMultiplier(string name, GridTypeThreatMultiplier multipliers)
            {
                Name = name;
                SmallGridMultiplier = multipliers.SmallGridMultiplier;
                LargeGridMultiplier = multipliers.LargeGridMultiplier;
                StationMultiplier = multipliers.StationMultiplier;
            }

            public void UpdateFrom(GridTypeThreatMultiplier multipliers)
            {
                multipliers.SmallGridMultiplier = SmallGridMultiplier;
                multipliers.LargeGridMultiplier = LargeGridMultiplier;
                multipliers.StationMultiplier = StationMultiplier;
            }
        }

        public enum SyncDirection
        {
            FromConfig, // config → bindings
            ToConfig    // bindings → config
        }

        private void SyncMultipliers(ThreatSettings config, SyncDirection direction)
        {
            foreach (var named in multiplierList)
            {
                switch (named.Name)
                {
                    case "GridTypeThreatMultiplier":
                        if (direction == SyncDirection.FromConfig)
                        {
                            named.SmallGridMultiplier = config.GridTypeMultipliers.SmallGridMultiplier;
                            named.LargeGridMultiplier = config.GridTypeMultipliers.LargeGridMultiplier;
                            named.StationMultiplier = config.GridTypeMultipliers.StationMultiplier;
                        }
                        else
                        {
                            named.UpdateFrom(config.GridTypeMultipliers);
                        }
                        break;

                    case "PowerOutputMultipliers":
                        if (direction == SyncDirection.FromConfig) {
                            named.SmallGridMultiplier = config.GridPowerOutputMultipliers.SmallGridMultiplier;
                            named.LargeGridMultiplier = config.GridPowerOutputMultipliers.LargeGridMultiplier;
                            named.StationMultiplier = config.GridPowerOutputMultipliers.StationMultiplier;
                        }
                        else
                        {
                            named.UpdateFrom(config.GridPowerOutputMultipliers);
                        }
                           
                        break;

                    case "BoundingBoxSizeMultiplier":
                        if (direction == SyncDirection.FromConfig)
                        {
                            named.SmallGridMultiplier = config.BoundingBoxSizeMultipliers.SmallGridMultiplier;
                            named.LargeGridMultiplier = config.BoundingBoxSizeMultipliers.LargeGridMultiplier;
                            named.StationMultiplier = config.BoundingBoxSizeMultipliers.StationMultiplier;
                        }
                        else
                        {
                            named.UpdateFrom(config.BoundingBoxSizeMultipliers);
                        }
                        break;

                    case "ThreatPerBlockMultiplier":
                        if (direction == SyncDirection.FromConfig)
                        {
                            named.SmallGridMultiplier = config.ThreatPerBlockMultipliers.SmallGridMultiplier;
                            named.LargeGridMultiplier = config.ThreatPerBlockMultipliers.LargeGridMultiplier;
                            named.StationMultiplier = config.ThreatPerBlockMultipliers.StationMultiplier;
                        }
                        else
                        {
                            named.UpdateFrom(config.ThreatPerBlockMultipliers);
                        }
                        break;
                }
            }
        }

        BindingList<NamedMultiplier> multiplierList;

        public MESHelperConfigvIEWManager(MESHelperState state)
        {
            _state = state;       
            _state.OnMainFormLoaded += ((o) => { forms_loaded_init(); return 0; });
        }


        private void SyncMultiplierBindingsToConfig() => SyncMultipliers(CurrentThreatConfig, SyncDirection.ToConfig);
        private void SyncMultiplierindingsFromConfig(ThreatSettings config) => SyncMultipliers(config, SyncDirection.FromConfig);

       
        
        private void forms_loaded_init()
        {
            CategoryTable.RowValidating += validateCategory;
            BlockTable.RowValidating += validateBlock;

            CategoryTable.DataSource = categoryBinding;
            BlockTable.DataSource = blockBinding;

            BlockTable.CellValueChanged += BlockTable_CellValueChanged;
            CategoryTable.CellValueChanged += CategoryTable_CellValueChanged;
            MultipliersTable.CellValueChanged += MultipliersTable_CellValueChanged;


            _state.OnCurrentDocumentChanged += _state_OnCurrentDocumentChanged;

            _state.OnDocumentLoadedFromXML += _state_DocumentLoadedFromXML;

            _state.OnChangedThreatConfigState += _state_OnChangedConfigState1;

            _state.OnRequestConfigFromDocumentSync += _state_OnRequestConfigFromDocumentSync;

            BlockTable.UserDeletingRow += BlockTable_UserDeletingRow; ;
        }

        private void BlockTable_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            SyncBindingsToConfig();
            if (CurrentThreatConfig != null)
                _state.TriggerChangedThreatConfigState(this, CurrentThreatConfig, null);
            _state.RequestConfigToDocumentSync();
        }

        private void CategoryTable_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            SyncBindingsToConfig();
            if (CurrentThreatConfig != null)
                _state.TriggerChangedThreatConfigState(this, CurrentThreatConfig, null);
            _state.RequestConfigToDocumentSync();
        }

        private void BlockTable_UserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
        {
            var rowItem = e.Row.DataBoundItem as SingleBlockThreat;
            if (rowItem != null)
            {
                this.CurrentThreatConfig.SingleBlockThreatEntries.Remove(rowItem);
                _state.RequestConfigToDocumentSync();
            }
        }

  
        private void _state_OnRequestConfigFromDocumentSync()
        {
            ReloadConfigurationFromDocument();
        }

        private void _state_OnChangedConfigState1(object? sender, Event.ChangedConfigEventArgs e)
        {
            SyncMultiplierindingsFromConfig(e.CurrentConfig);
            SyncThreatBindingsFromConfig(e.CurrentConfig.BlockCategoryThreatEntries, e.CurrentConfig.SingleBlockThreatEntries);           
        }

        private void _state_DocumentLoadedFromXML(object? sender, Event.DocumentLoadedEventArgs e)
        {
            categoryBinding.Clear();
            blockBinding.Clear();
            if (multiplierList != null) multiplierList.Clear() ;
            ReloadConfigurationFromDocument();
        }

        private void MultipliersTable_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            SyncMultiplierBindingsToConfig();
            _state.RequestConfigToDocumentSync();
        }


        private int _state_OnCurrentDocumentChanged(object sender, XDocument? previousDocument, XDocument? newDocument)
        {            
            return 0;
        }


        private int _state_OnCurrentDocumentLoad(object sender, XDocument? newDocument)
        {

          
          
            return 1;
        }

        public void ProcessSbcFile(string filePath)
        {

            if (CurrentThreatConfig == null) return;
            var doc = XDocument.Load(filePath);
            var definitions = doc.Descendants("Definition");
            List<SingleBlockThreat> queue = new List<SingleBlockThreat>();
            foreach (var def in definitions)
            {
                string typeId = def.Descendants("TypeId").FirstOrDefault()?.Value?.Trim();
                string subtypeId = def.Descendants("SubtypeId").FirstOrDefault()?.Value?.Trim();

                Console.WriteLine($"Found: TypeId = {typeId}, SubtypeId = {subtypeId}");
                if (string.IsNullOrWhiteSpace(typeId) && string.IsNullOrWhiteSpace(subtypeId))
                    continue;

                bool alreadyExists = CurrentThreatConfig.SingleBlockThreatEntries
                    .Any(e =>
                        string.Equals(e.BlockType, typeId, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(e.BlockSubType, subtypeId, StringComparison.OrdinalIgnoreCase)
                    );

                if (!alreadyExists) queue.Add(new SingleBlockThreat() { BlockSubType = subtypeId, BlockType = typeId});

            }

            if(queue.Count > 0)
            {
                var copy = CurrentThreatConfig.copy();
                CurrentThreatConfig.SingleBlockThreatEntries.AddRange(queue);     
              
                ChangedConfigState(this, CurrentThreatConfig, copy);
                _state.RequestConfigToDocumentSync();
            }
           
        }

        private void ChangedConfigState(
            MESHelperConfigvIEWManager mESHelperConfigManager, 
            ThreatSettings currentThreatConfig,
            ThreatSettings? oldThreatConfig) => _state.TriggerChangedThreatConfigState(mESHelperConfigManager, currentThreatConfig, oldThreatConfig);

        
        private static ThreatSettings DeserializeConfig(string xmlContent)
        {
            var serializer = new XmlSerializer(typeof(ThreatSettings));
            using var reader = new StringReader(xmlContent);
            return (ThreatSettings)serializer.Deserialize(reader);
        }
        private void ReloadConfigurationFromDocument()
        {
   
            var serializer = new XmlSerializer(typeof(ThreatSettings));
            var reader = _state.CurrentXmlDocument.CreateReader();

            var newConfig = serializer.Deserialize(reader) as ThreatSettings;
            var oldConfig = CurrentThreatConfig != null ? CurrentThreatConfig.copy() : null;

            if (newConfig == null) return;
            if (multiplierList == null || multiplierList.Count < 4)
            {
                multiplierList = new BindingList<NamedMultiplier>() {
                        new NamedMultiplier("GridTypeThreatMultiplier", newConfig.GridTypeMultipliers),
                        new NamedMultiplier("PowerOutputMultipliers", newConfig.GridPowerOutputMultipliers),
                        new NamedMultiplier("BoundingBoxSizeMultiplier", newConfig.BoundingBoxSizeMultipliers),
                        new NamedMultiplier("ThreatPerBlockMultiplier", newConfig.ThreatPerBlockMultipliers),
                };
                MultipliersTable.DataSource = multiplierList;
            }           

                _state.CurrentThreatConfiguration = newConfig;
            SyncThreatBindingsFromConfig(newConfig.BlockCategoryThreatEntries, newConfig.SingleBlockThreatEntries, true);
            ChangedConfigState(this, _state.CurrentThreatConfiguration, oldConfig);
        }

        private void SyncThreatBindingsFromConfig(List<BlockCategoryThreat> blockCategoryThreatEntries, List<SingleBlockThreat> singleBlockThreatEntries, bool force_clear = false)
        {
            if (_state == null || CurrentThreatConfig == null) return;
                        
            if (force_clear)
            {
                categoryBinding.Clear();
                blockBinding.Clear();
            }
            foreach (var cat in blockCategoryThreatEntries.Where((item) =>!categoryBinding.Contains(item)))
                categoryBinding.Add(cat);         
            foreach (var cat in categoryBinding.Where((item) => !blockCategoryThreatEntries.Contains(item)))
                categoryBinding.Remove(cat);

            foreach (var blk in singleBlockThreatEntries.Where((item) => !blockBinding.Contains(item)))
                blockBinding.Add(blk);
            foreach (var blk in blockBinding.Where((item) => !singleBlockThreatEntries.Contains(item)))
                blockBinding.Remove(blk);
        }
        private void SyncBindingsToConfig()
        {
            if (_state == null || CurrentThreatConfig == null) return;

            // ===== Categories =====
            CurrentThreatConfig.BlockCategoryThreatEntries.Clear();
            foreach (var cat in categoryBinding)
            {
                if (cat is BlockCategoryThreat catEntry)
                    CurrentThreatConfig.BlockCategoryThreatEntries.Add(catEntry);
            }

            // ===== Blocks =====
            CurrentThreatConfig.SingleBlockThreatEntries.Clear();
            foreach (var blk in blockBinding)
            {
                if (blk is SingleBlockThreat blkEntry)
                    CurrentThreatConfig.SingleBlockThreatEntries.Add(blkEntry);
            }
        }

        private float TryParseFloat(object value)
        {
            if (value == null) return 0.0f;
            if (float.TryParse(value.ToString(), out float result)) return result;
            return 0.0f;
        }

        private int TryParseInt(object value)
        {
            if (value == null) return 0;
            if (int.TryParse(value.ToString(), out int result)) return result;
            return 0;
        }

        private void validateCategory(object sender, DataGridViewCellCancelEventArgs e)
        {
            var dgv = (DataGridView)sender;

            // Check if this is the new row being edited
            if (dgv.Rows[e.RowIndex].IsNewRow)
                return;

            var row = dgv.Rows[e.RowIndex];

            // Get the current object bound to this row (if any)
            if (row.DataBoundItem == null)
            {
                // Create a new BlockCategoryThreat from the row cell values
                var newCategory = new BlockCategoryThreat
                {
                    Category = row.Cells["Category"].Value?.ToString() ?? "",
                    Threat = TryParseFloat(row.Cells["Threat"].Value),
                    Multiplier = TryParseFloat(row.Cells["Multiplier"].Value),
                    MultiplierThreshold = TryParseInt(row.Cells["Threshold"].Value),
                    FullVolumeThreat = TryParseFloat(row.Cells["FullVolumeThreat"].Value),
                };

                // Add to the BindingList
                categoryBinding.Add(newCategory);
            }
        }

        private void validateBlock(object sender, DataGridViewCellCancelEventArgs e)
        {
            var dgv = (DataGridView)sender;

            if (dgv.Rows[e.RowIndex].IsNewRow)
                return;

            if (dgv.Rows.Count < e.RowIndex)
                return;

            var row = dgv.Rows[e.RowIndex];

            if (row.DataBoundItem == null)
            {
                var newBlock = new SingleBlockThreat
                {
                    BlockType = row.Cells["BlockType"].Value?.ToString() ?? "",
                    BlockSubType = row.Cells["BlockSubType"].Value?.ToString() ?? "",
                    Threat = TryParseFloat(row.Cells["Threat"].Value),
                    Multiplier = TryParseFloat(row.Cells["Multiplier"].Value),
                    MultiplierThreshold = TryParseInt(row.Cells["Threshold"].Value),
                    FullVolumeThreat = TryParseFloat(row.Cells["FullVolumeThreat"].Value),
                };

                blockBinding.Add(newBlock);
            }
        }

      
    }
}
