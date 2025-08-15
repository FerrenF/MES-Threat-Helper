using MESHelper.Configuration;
using MESHelper.Event;
using MESHelper.Threat.Core;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MESHelper
{
    public class MESHelperDocumentManager
    {

        private MESHelperState CurrentState;
        private bool HasCurrentXmlDocument => CurrentState != null && CurrentState.CurrentXmlDocument != null;

        private string _currentDocumentPath = string.Empty;

        public string CurrentDocumentPath {  get { return _currentDocumentPath; } }
        private XDocument? CurrentXmlDocument => HasCurrentXmlDocument ? CurrentState.CurrentXmlDocument : null;
        private Scintilla? CurrentXMLEditor => CurrentState.MainViewInstance?.xmlEditor;

        public MESHelperDocumentManager(MESHelperState state)
        {
            CurrentState = state;
            CurrentState.OnMainFormLoaded += ((o) => { forms_loaded_init(); return 0; });
  
        }

        private void CurrentState_DocumentLoadedFromXML(object? sender, DocumentLoadedEventArgs e)
        {
            if( e.FilePath == null)
            {

                return;
            }
            CurrentState.AppConfig.AddRecentFile(e.FilePath);
        }   

        public void forms_loaded_init()
        {
            CurrentXMLEditor.TextChanged += CurrentXMLEditor_TextChanged1;
            CurrentXMLEditor.LostFocus += CurrentXMLEditor_LostFocus;
            CurrentState.OnChangedThreatConfigState += CurrentState_OnChangedConfigState;
            CurrentState.OnDocumentLoadedFromXML += CurrentState_DocumentLoadedFromXML;
            CurrentState.OnRequestDocumentFromConfigSync += CurrentState_OnRequestDocumentSync;
        }

        private void CurrentXMLEditor_LostFocus(object? sender, EventArgs e)
        {
            if (has_text_changed_for_refresh) CurrentState.RequestDocumentToConfigSync();
        }

        private void CurrentState_OnRequestDocumentSync()
        {
            UpdateFromConfigThreat(CurrentState.CurrentThreatConfiguration);
            CurrentXMLEditor.Text = FormatXml(CurrentState.CurrentXmlDocument.ToString());
        }

        private void CurrentState_OnChangedConfigState(object? sender, ChangedConfigEventArgs e)
        {
            UpdateFromConfigThreat(e.CurrentConfig);
            CurrentXMLEditor.Text = FormatXml(CurrentState.CurrentXmlDocument.ToString());
        }

        bool has_text_changed_for_refresh = false;
        private void CurrentXMLEditor_TextChanged1(object? sender, EventArgs e)
        {
            has_text_changed_for_refresh = true;
        }

        private int State_OnCurrentDocumentSet(object sender, XDocument? newDocument)
        {

            UpdateEditorUI();
            return 1;
        }


        // ====== BLOCKS ======

        public XElement? GetBlock(string type, string subtype)
        {
            return CurrentXmlDocument?
                .Root?
                .Element("BlockThreat")?
                .Elements("Block")
                .FirstOrDefault(b =>
                    string.Equals((string)b.Attribute("Type"), type, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((string)b.Attribute("SubType"), subtype, StringComparison.OrdinalIgnoreCase));
        }

        public void SetBlockAttribute(string type, string subtype, string attributeName, string value)
        {
            var block = GetBlock(type, subtype);
            if (block != null)
            {
                block.SetAttributeValue(attributeName, value);
            }
            else
            {
                // Optionally create if not found
                var newBlock = new XElement("Block",
                    new XAttribute("Type", type),
                    new XAttribute("SubType", subtype),
                    new XAttribute(attributeName, value));
                CurrentXmlDocument?.Root?.Element("BlockThreat")?.Add(newBlock);
            }
        }

        // ====== CATEGORIES ======

        public XElement? GetCategory(string name)
        {
            return CurrentXmlDocument?
                .Root?
                .Element("CategoryThreat")?
                .Elements("Category")
                .FirstOrDefault(c =>
                    string.Equals(c.Value, name, StringComparison.OrdinalIgnoreCase));
        }

        public void SetCategoryAttribute(string name, string attributeName, string value)
        {
            var category = GetCategory(name);
            if (category != null)
            {
                category.SetAttributeValue(attributeName, value);
            }
        }

        // ====== MULTIPLIERS ======

        public XElement? GetMultiplierElement(string sectionName, string multiplierName)
        {
            return CurrentXmlDocument?
                .Root?
                .Element(sectionName)?
                .Element(multiplierName);
        }

        public void SetMultiplierValue(string sectionName, string multiplierName, string value)
        {
            var multiplier = GetMultiplierElement(sectionName, multiplierName);
            if (multiplier != null)
            {
                multiplier.Value = value;
            }
        }


        public void UpdateFromConfigThreat(ConfigThreat config)
        {
            var root = CurrentXmlDocument?.Root;
            if (root == null) return;

            // ===== Blocks =====
            var blockThreatElem = root.Element("BlockThreat");
            if (blockThreatElem != null)
            {
                // Remove blocks not in config
                var blocksToRemove = blockThreatElem.Elements("Block")
                    .Where(e => !config.SingleBlockThreatEntries.Any(entry =>
                        string.Equals((string)e.Attribute("Type"), entry.BlockType, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals((string)e.Attribute("SubType"), entry.BlockSubType, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                foreach (var b in blocksToRemove) b.Remove();

                // Add/update blocks from config
                foreach (var entry in config.SingleBlockThreatEntries)
                {
                    var blockElem = blockThreatElem.Elements("Block")
                        .FirstOrDefault(e =>
                            string.Equals((string)e.Attribute("Type"), entry.BlockType, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals((string)e.Attribute("SubType"), entry.BlockSubType, StringComparison.OrdinalIgnoreCase));

                    if (blockElem != null)
                    {
                        blockElem.SetAttributeValue("Threat", entry.Threat);
                        blockElem.SetAttributeValue("Multiplier", entry.Multiplier);
                        blockElem.SetAttributeValue("MultiplierThreshold", entry.MultiplierThreshold);
                        blockElem.SetAttributeValue("FullVolumeThreat", entry.FullVolumeThreat);
                    }
                    else
                    {
                        blockThreatElem.Add(new XElement("Block",
                            new XAttribute("Type", entry.BlockType ?? string.Empty),
                            new XAttribute("SubType", entry.BlockSubType ?? string.Empty),
                            new XAttribute("Threat", entry.Threat),
                            new XAttribute("Multiplier", entry.Multiplier),
                            new XAttribute("MultiplierThreshold", entry.MultiplierThreshold),
                            new XAttribute("FullVolumeThreat", entry.FullVolumeThreat)
                        ));
                    }
                }
            }

            // ===== Categories =====
            var categoryThreatElem = root.Element("CategoryThreat");
            if (categoryThreatElem != null)
            {
                // Remove categories not in config
                var categoriesToRemove = categoryThreatElem.Elements("Category")
                    .Where(e => !config.BlockCategoryThreatEntries.Any(entry =>
                        string.Equals(e.Value?.Trim(), entry.Category, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                foreach (var c in categoriesToRemove) c.Remove();

                // Add/update categories from config
                foreach (var entry in config.BlockCategoryThreatEntries)
                {
                    var catElem = categoryThreatElem.Elements("Category")
                        .FirstOrDefault(e =>
                            string.Equals(e.Value?.Trim(), entry.Category, StringComparison.OrdinalIgnoreCase));

                    if (catElem != null)
                    {
                        catElem.SetAttributeValue("Threat", entry.Threat);
                        catElem.SetAttributeValue("Multiplier", entry.Multiplier);
                        catElem.SetAttributeValue("MultiplierThreshold", entry.MultiplierThreshold);
                        catElem.SetAttributeValue("FullVolumeThreat", entry.FullVolumeThreat);
                    }
                    else
                    {
                        categoryThreatElem.Add(new XElement("Category",
                            new XAttribute("Threat", entry.Threat),
                            new XAttribute("Multiplier", entry.Multiplier),
                            new XAttribute("MultiplierThreshold", entry.MultiplierThreshold),
                            new XAttribute("FullVolumeThreat", entry.FullVolumeThreat),
                            entry.Category ?? string.Empty
                        ));
                    }
                }
            }

            // ===== Grid Type Multipliers =====
            UpdateMultiplierSection(root.Element("GridTypeThreatMultiplier"), config.GridTypeMultipliers);
            UpdateMultiplierSection(root.Element("PowerOutputMultipliers"), config.GridPowerOutputMultipliers);
            UpdateMultiplierSection(root.Element("BoundingBoxSizeMultiplier"), config.BoundingBoxSizeMultipliers);
            UpdateMultiplierSection(root.Element("ThreatPerBlockMultiplier"), config.ThreatPerBlockMultipliers);

            // ===== Mod Version =====
            var modVersionElem = root.Element("ThreatModVersion");
            if (modVersionElem != null)
            {
                modVersionElem.Value = config.ThreatModVersion ?? string.Empty;
            }
            else
            {
                root.Add(new XElement("ThreatModVersion", config.ThreatModVersion ?? string.Empty));
            }

            CurrentXMLEditor.Text = FormatXml(CurrentXmlDocument.ToString());
        }



        // Helper for updating <SmallGridMultiplier>, <LargeGridMultiplier>, <StationMultiplier>
        private void UpdateMultiplierSection(XElement? sectionElem, GridTypeThreatMultiplier data)
        {
            if (sectionElem == null) return;

            sectionElem.SetElementValue("SmallGridMultiplier", data.SmallGridMultiplier);
            sectionElem.SetElementValue("LargeGridMultiplier", data.LargeGridMultiplier);
            sectionElem.SetElementValue("StationMultiplier", data.StationMultiplier);
        }

        public int GetCategoryLine(string categoryName)
        {
            if (!HasCurrentXmlDocument || CurrentXmlDocument == null) return -1;

            var element = CurrentXmlDocument
             .Descendants("Category")
             .FirstOrDefault(e => (e.Nodes().OfType<XText>().FirstOrDefault()?.Value.Trim() ?? "") == categoryName);

            if (element == null) return -1;

            var info = (IXmlLineInfo)element;
            return info.HasLineInfo() ? info.LineNumber - 1: -1;
        }

        public int GetBlockLine(string blockType, string blockSubType)
        {
            if (!HasCurrentXmlDocument || CurrentXmlDocument == null) return -1;

            var blockMatches = CurrentXmlDocument
                .Descendants("Block")
                .Where(e =>
                    (e.Attribute("Type")?.Value?.Trim() ?? "") == blockType &&
                    (e.Attribute("SubType")?.Value?.Trim() ?? "") == blockSubType
                );

            var match = blockMatches.FirstOrDefault();
            if (match == null) return -1;

            var info = (IXmlLineInfo)match;
            return info.HasLineInfo() ? info.LineNumber - 1 : -1;
        }

        public int GetMultiplierLine(string parentElementName, string multiplierName)
        {
            if (!HasCurrentXmlDocument || CurrentXmlDocument == null) return -1;

            var parent = CurrentXmlDocument.Element("ThreatSettings")?.Element(parentElementName);
            if (parent == null) return -1;

            var multiplierElem = parent.Element(multiplierName);
            if (multiplierElem == null) return -1;

            var info = (IXmlLineInfo)multiplierElem;
            return info.HasLineInfo() ? info.LineNumber - 1 : -1;
        }

        public bool LoadXml(string filePath)
        {
            bool loadSuccess = false;
            string loadedContent = "";

            try
            {
                string xmlContent = File.ReadAllText(filePath);
                loadedContent = FormatXml(xmlContent);
                _currentDocumentPath = filePath;
                if (xmlContent == "")
                {
                    throw new Exception($"Failed to format XML filed {filePath}");
                }             
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load XML: {ex.Message}");
            }
         

            if (loadedContent == "") return false;         
   
            CurrentState.CurrentXmlDocument = XDocument.Parse(loadedContent, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
            if (CurrentXMLEditor != null) CurrentXMLEditor.Text = CurrentState.CurrentXmlDocument.ToString();
            TriggerDocumentLoadedFromXML(this, filePath, CurrentState.CurrentXmlDocument);
            return true;
        }

        private void TriggerDocumentLoadedFromXML(MESHelperDocumentManager mESHelperDocumentManager,string filePath,
      XDocument currentXmlDocument) => CurrentState.TriggerDocumentLoadedFromXML(mESHelperDocumentManager, filePath, currentXmlDocument);
     


        public void UpdateEditorUI()
        {
            LoadStringXml(CurrentXMLEditor.Text, false);
        }


        public bool LoadStringXml(string xml, bool updateEditor = true)
        {
            bool loadSuccess = false;
            string loadedContent = "";

            try
            {
                loadedContent = FormatXml(xml);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load XML from string: {ex.Message}");
            }

            if (loadedContent == "") return false;

            CurrentState.CurrentXmlDocument = XDocument.Parse(loadedContent, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
            if (CurrentXMLEditor != null && updateEditor) CurrentXMLEditor.Text = CurrentState.CurrentXmlDocument.ToString();
            return true;
        }

        private string FormatXml(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                using var stringWriter = new StringWriter();
                using var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };
                doc.Save(xmlTextWriter);
                return stringWriter.ToString();
            }
            catch
            {            
                return "";
            }
        }

        internal void SaveCurrentDocument()
        {
            if (CurrentDocumentPath == string.Empty)
            {
                using SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, CurrentXMLEditor?.Text.ToString());
                }
            }
            else
            {
                File.WriteAllText(CurrentDocumentPath, CurrentXMLEditor?.Text.ToString());
            }
        }
    }
}
