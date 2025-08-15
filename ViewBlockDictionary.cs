using MESHelper.Threat;
using MESHelper.Threat.Core;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using static MESHelper.BlueprintParser;

namespace MESHelper
{
    [XmlRoot("BlockDictionaryEntry")]
    public class BlockDictionaryEntry

    {
        public enum BlockSize
        {
            Small,
            Large
        }

        [JsonInclude]
        [XmlAttribute("Type")]
        public string Type { get; set; } = string.Empty;

        [JsonInclude]
        [XmlAttribute("SubType")]
        public string SubType { get; set; } = string.Empty;

        [JsonInclude]
        [XmlAttribute("Category")]
        public string Category { get; set; } = string.Empty;

        [JsonInclude]
        [XmlAttribute("MaxPowerOut")]
        public float MaxPowerOut { get; set; } = 0;

        [JsonInclude]
        [XmlAttribute("MaxPowerIn")]
        public float MaxPowerIn { get; set; } = 0;

        [JsonInclude]
        [XmlAttribute("MinPowerIn")]
        public float MinPowerIn { get; set; } = 0;

        [JsonInclude]
        [XmlAttribute("Scale")]
        public float Scale { get; set; } = 0.0f;

        [JsonInclude]
        [XmlAttribute("MaxVolume")]
        public float MaxVolume { get; set; } = 0;

        [JsonInclude]
        [XmlAttribute("Size")]
        public BlockSize Size { get; set; } = BlockSize.Large;

    }

    public partial class ViewBlockDictionary : Form
    {

        public DataGridView BlockDictionaryDisplay;
        public MESHelperState? CurrentState = MESHelperState.Instance;

        private SortableBindingList<BlockDictionaryEntry> _entries = new SortableBindingList<BlockDictionaryEntry>();
        
        private Button _btnSelectDirectory;
        private Button _btnClearDictionary;
        public ViewBlockDictionary()
        {
            InitializeComponents();
            if (CurrentState != null)
            {
                CurrentState.ViewBlockDictionaryInstance = this;
                CurrentState.BlockDictionaryViewLoaded(this);
            }

            if (CurrentState.AppConfig != null)
                LoadFromState(CurrentState.AppConfig.BlockDictionary);
            else
                CurrentState.OnAppConfigLoaded += CurrentState_OnAppConfigLoaded;          
        }

        private void CurrentState_OnAppConfigLoaded(object? sender, Event.ConfigLoadedEventArgs e)
        {
            if (e.ConfigTypeLoaded == Event.ConfigLoadedEventArgs.ConfigType.AppConfig)
                LoadFromState(CurrentState.AppConfig.BlockDictionary);
        }

        private void InitializeComponents()
        {
            this.Text = "Block Dictionary Viewer";
            this.Width = 1200;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeBlockDictionaryDisplay();
            InitializeSelectPanel();
          
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _btnSelectDirectory.Click += BtnSelectDirectory_Click;
            _btnClearDictionary.Click += _btnClearDictionary_Click;
            this.FormClosing += ViewBlockDictionary_FormClosing;
        }

        private void _btnClearDictionary_Click(object? sender, EventArgs e)
        {
            _entries.Clear();
            CurrentState.AppConfig.ResetBlockDictionary(this);            
        }

        private void InitializeSelectPanel()
        {
            var selectPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                
                Height = 40,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5)
            };

            _btnSelectDirectory = new Button
            {
                Text = "Import Directory",
                AutoSize = true
            };
            _btnClearDictionary = new Button
            {
                Text = "Clear",
                AutoSize = true
            };
            selectPanel.Controls.AddRange([_btnSelectDirectory, _btnClearDictionary]);
            this.Controls.Add(selectPanel);
        }

        private void InitializeBlockDictionaryDisplay()
        {
            BlockDictionaryDisplay = new DataGridView
            {                
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                DataSource = _entries,
                AllowUserToOrderColumns = true                
            };

            BlockDictionaryDisplay.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Width = 200,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            BlockDictionaryDisplay.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SubType",
                HeaderText = "SubType",
                Width = 200,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            BlockDictionaryDisplay.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Category",
                HeaderText = "Category",
                Width = 120,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            BlockDictionaryDisplay.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaxPowerOut",
                HeaderText = "Max Power Output",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            BlockDictionaryDisplay.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaxPowerIn",
                HeaderText = "Max Power Input",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            BlockDictionaryDisplay.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MinPowerIn",
                HeaderText = "Min Power Input",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            BlockDictionaryDisplay.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaxVolume",
                HeaderText = "Max Volume",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            BlockDictionaryDisplay.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Size",
                HeaderText = "Size",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
            BlockDictionaryDisplay.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Scale",
                HeaderText = "Scale",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.Automatic
            });

            this.Controls.Add(BlockDictionaryDisplay);
        }

        private void BtnSelectDirectory_Click(object? sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a directory to scan for block definitions";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    ScanForBlockDefinitions(folderDialog.SelectedPath);

                    CurrentState.AppConfig.SaveBlockDictionary(_entries.ToList());
                }
            }
        }

        private float magic_number = 125f;
        private float inventoryVolumeFrom(Vector3 size, BlockDictionaryEntry.BlockSize blockType)
        {
            float mult = 1f;
            if(blockType == BlockDictionaryEntry.BlockSize.Small)
                return (size.X * size.Y * size.Z) * magic_number;
            else{
                return ((size.X * 5) * (size.Y * 5) * (size.Z * 5)) * magic_number;
            }
        }
        private void ScanForBlockDefinitions(string directory)
        {      
            var existing = _entries.ToDictionary(e => (e.Type, e.SubType), e => e);

            foreach (var file in Directory.EnumerateFiles(directory, "*.sbc", SearchOption.AllDirectories))
            {
                try
                {
                    var doc = XDocument.Load(file);

                    var cubeBlocks = doc.Root?.Element("CubeBlocks");
                    if (cubeBlocks == null)
                        continue;

                    foreach (var def in cubeBlocks.Elements("Definition"))
                    {
                        var typeId = def.Element("Id")?.Element("TypeId")?.Value ?? string.Empty;
                        var subtypeId = def.Element("Id")?.Element("SubtypeId")?.Value ?? string.Empty;
                        if (string.IsNullOrEmpty(typeId) || string.IsNullOrEmpty(subtypeId))
                            continue;

                        var cat = new Threat.CategoryProvider.CategoryGuesstimator(MESHelperState.Instance.AppConfig.BlockDictionary).GetCategory(def);
                        if (cat.Equals(string.Empty)) continue;

                        var key = (typeId, subtypeId);
                        if (!existing.TryGetValue(key, out var entry))
                        {
                            entry = new BlockDictionaryEntry
                            {
                                Type = typeId,
                                SubType = subtypeId
                            };
                            existing[key] = entry;
                        }

                        entry.Category = cat;
                        entry.Size = ParseCubeSize(def.Element("CubeSize")?.Value) ?? BlockDictionaryEntry.BlockSize.Large;


                        float? inventoryVolume = ParseFloat(def.Element("InventoryMaxVolume")?.Value);
                        float? capacity = ParseFloat(def.Element("Capacity")?.Value);
                        var invSizeElement = def.Element("InventorySize");
                        Vector3? scale = ParseVec3(def.Element("Size"));

                        if (scale.HasValue)
                            entry.Scale = scale.Value.LengthSquared();

                        if (capacity.HasValue)
                            entry.MaxVolume = capacity.Value;

                        else if (inventoryVolume.HasValue)
                            entry.MaxVolume = inventoryVolume.Value * (entry.Size == BlockDictionaryEntry.BlockSize.Large ? 5 : 1) * magic_number;
                        else if (invSizeElement != null)
                        {
                            float? x = ParseFloat(invSizeElement.Element("X")?.Value);
                            float? y = ParseFloat(invSizeElement.Element("Y")?.Value);
                            float? z = ParseFloat(invSizeElement.Element("Z")?.Value);
                            if (x.HasValue && y.HasValue && z.HasValue)
                                entry.MaxVolume = inventoryVolumeFrom(new Vector3(x.Value, y.Value, z.Value), entry.Size);
                        }
                        else if (new string[] {"cargo","container","welder","grinder","drill"}.Where( (st)=>typeId.ToLower().Contains(st)).Count() != 0)
                        {
                            if (scale.HasValue)
                                entry.MaxVolume = inventoryVolumeFrom(scale.Value, entry.Size);
                        }

                        #region Power_Related

                        float? maxPowerOutput = ParseFloat(def.Element("MaxPowerOutput")?.Value);
                        float? maxPowerInput = ParseFloat(def.Element("MaxPowerConsumption")?.Value);
                        float? minPowerInput = ParseFloat(def.Element("MinPowerConsumption")?.Value);
                        float? maxPowerConsumption = ParseFloat(def.Element("OperationalPowerConsumption")?.Value);

                        if (maxPowerOutput.HasValue)
                            entry.MaxPowerOut = maxPowerOutput.Value;

                        if (maxPowerInput.HasValue)
                            entry.MaxPowerIn = maxPowerInput.Value;
                        else if (maxPowerConsumption.HasValue)
                            entry.MaxPowerIn = maxPowerInput.Value;


                        float? standbyPowerConsumption = ParseFloat(def.Element("StandbyPowerConsumption")?.Value);
                        if (minPowerInput.HasValue)
                            entry.MinPowerIn = minPowerInput.Value;
                        else if (standbyPowerConsumption.HasValue)
                               entry.MinPowerIn = standbyPowerConsumption.Value;
      

                        #endregion
                       
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing {file}: {ex.Message}");
                }
            }
            _entries.Clear();
            foreach (var entry in existing.Values){
                _entries.Add(entry);
            }
            CurrentState.AppConfig.SaveBlockDictionary(_entries.ToList());
        }

        private Vector3? ParseVec3(XElement element)
        {
            float x = ParseFloat(element.Attribute("x")?.Value) ?? 0.5f;
            float y = ParseFloat(element.Attribute("y")?.Value) ?? 0.5f;
            float z = ParseFloat(element.Attribute("z")?.Value) ?? 0.5f;
            return new Vector3(x, y, z);
        }

        private BlockDictionaryEntry.BlockSize? ParseCubeSize(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return value.Equals("Small", StringComparison.OrdinalIgnoreCase)
                ? BlockDictionaryEntry.BlockSize.Small
                : BlockDictionaryEntry.BlockSize.Large;
        }
        private double? ParseDouble(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }
        private float? ParseFloat(string value)
        {
            if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }
        // Allow external code to refresh display from MESHelperState
        public void LoadFromState(IEnumerable<BlockDictionaryEntry> entries)
        {
            _entries.Clear();
            foreach (var entry in entries)
                _entries.Add(entry);
        }
        private void ViewBlockDictionary_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Save current dictionary to config when window closes
            CurrentState.AppConfig.SaveBlockDictionary(_entries.ToList());
        }
    }
}
