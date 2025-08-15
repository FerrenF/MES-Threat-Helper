using MESHelper.Configuration;
using MESHelper.Threat.Core;
using ScintillaNET;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using VPKSoft.ScintillaLexers;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Timer = System.Windows.Forms.Timer;

namespace MESHelper
{
    partial class MainView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private ConfigThreat currentThreatConfiguration;
        public Scintilla xmlEditor;
        public DataGridView CategoryTable;
        public DataGridView BlockTable;
        private SplitContainer rightTableSplitContainer;

        private SplitContainer mainViewSplitContainer;
        private SplitContainer rightMainSplitContainer;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem editMenu;
        private ToolStripMenuItem viewMenu;
        public DataGridView GridTypeTable;  
        public MESHelperState State => MESHelperState.Instance; 


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void ConfigureXmlLexer()
        {
            xmlEditor.StyleResetDefault();
            xmlEditor.Styles[Style.Default].Font = "Consolas";
            xmlEditor.Styles[Style.Default].Size = 10;
            xmlEditor.StyleClearAll();

            xmlEditor.LexerName = "xml";
            xmlEditor.SetProperty("fold", "1"); // enable code folding

            xmlEditor.SetProperty("fold", "1");
            xmlEditor.SetProperty("fold.compact", "1");

            xmlEditor.Styles[(int)Style.Xml.Default].ForeColor = Color.Black;
            xmlEditor.Styles[(int)Style.Xml.Tag].ForeColor = Color.Blue;
            xmlEditor.Styles[(int)Style.Xml.Attribute].ForeColor = Color.DarkRed;
            xmlEditor.Styles[(int)Style.Xml.TagEnd].ForeColor = Color.DarkRed;
            xmlEditor.Styles[(int)Style.Xml.Value].ForeColor = Color.Maroon;
            xmlEditor.Styles[(int)Style.Xml.Comment].ForeColor = Color.Green;

            // Margins and line numbers
            xmlEditor.Margins[0].Type = MarginType.Number;
            xmlEditor.Margins[0].Width = 30;

            // Optionally set keywords if any custom tokens exist
        }
        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        /// 

        private void InitializeMenuStrip()
        {
            menuStrip = new MenuStrip();



            // File menu
            fileMenu = new ToolStripMenuItem("File");



            // Create Recent submenu (empty for now)
            var recentMenu = new ToolStripMenuItem("Recent");

            // When the File menu is about to open, refresh the recent list
            fileMenu.DropDownOpening += (s, e) =>
            {
                PopulateRecentMenu(recentMenu);
            };


            fileMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Open", null, OnOpenFile),
                new ToolStripMenuItem("Save", null, OnSaveFile),
                recentMenu,
                new ToolStripSeparator(),
                new ToolStripMenuItem("Exit", null, (s, e) => Close())
            });





            // Edit menu
            editMenu = new ToolStripMenuItem("Edit");
            editMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Undo", null, (s, e) => xmlEditor.Undo()),
                new ToolStripMenuItem("Redo", null, (s, e) => xmlEditor.Redo()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Cut", null, (s, e) => xmlEditor.Cut()),
                new ToolStripMenuItem("Copy", null, (s, e) => xmlEditor.Copy()),
                new ToolStripMenuItem("Paste", null, (s, e) => xmlEditor.Paste())
            });

            viewMenu = new ToolStripMenuItem("View");
            viewMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Threat Profiles", null, OnThreatProfilesClicked),
                new ToolStripMenuItem("Compare Profiles", null, OnCompareProfilesClicked),
                new ToolStripMenuItem("Block Dictionary", null, OnCompareBlocksClicked)
            });



            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, viewMenu });
            MainMenuStrip = menuStrip;
        }


        private void OnThreatProfilesClicked(object sender, EventArgs e)
        {
            State.ShowThreatProfileView();
        }

        private void OnCompareProfilesClicked(object sender, EventArgs e)
        {
            // TODO: Open Compare Profiles window
            MessageBox.Show("Compare Profiles clicked");
        }

        private void OnCompareBlocksClicked(object sender, EventArgs e)
        {
            State.ShowBlockDictionaryView();
        }

        void PopulateRecentMenu(ToolStripMenuItem recentMenuItem)
        {
            if (State.AppConfig == null) return;

            recentMenuItem.DropDownItems.Clear();
            if (State.AppConfig.RecentFiles != null && State.AppConfig.RecentFiles.Count > 0)
            {
                foreach (var filePath in State.AppConfig.RecentFiles)
                {
                    var displayName = Path.GetFileName(filePath);
                    var menuItem = new ToolStripMenuItem(displayName)
                    {
                        ToolTipText = filePath 
                    };

                    menuItem.Click += (s, e) => OnOpenRecentFile(filePath);
                    recentMenuItem.DropDownItems.Add(menuItem);
                }
            }
            else
            {
                recentMenuItem.DropDownItems.Add(new ToolStripMenuItem("(No recent files)")
                {
                    Enabled = false
                });
            }
        }

        // Handler for opening recent files
        private void OnOpenRecentFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                State.DocumentManager.LoadXml(filePath);
;           }
         
        }
        private void InitializeComponent()
        {
            SuspendLayout();

            // -------- MenuStrip --------

            InitializeMenuStrip();
            var mainContainer = new ToolStripContainer { Dock = DockStyle.Fill };
            mainContainer.TopToolStripPanel.Controls.Add(MainMenuStrip);


            // -------- MainSplitContainer --------
            mainViewSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = Width / 4
            };
                      
            rightTableSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = Height / 2
            };

            rightMainSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = Height / 2
            };

            // -------- XML Editor (Scintilla) --------
            xmlEditor = new Scintilla
            {
                Dock = DockStyle.Fill,
                WrapMode = WrapMode.None,
                Font = new Font("Consolas", 10),
                AllowDrop = true              
            };

            xmlEditor.DragEnter += xmlEditor_DragEnter;
            xmlEditor.DragDrop += xmlEditor_DragDrop;
            xmlEditor.TextChanged += XmlEditor_TextChanged;
            xmlEditor.UpdateUI += XmlEditor_UpdateUI;
            xmlEditor.LostFocus += XmlEditor_LostFocus;
            xmlEditor.KeyDown += XmlEditor_KeyDown;         
            // Table Display Config

            #region TableSetup

            CategoryTable = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 29,
                RowHeadersWidth = 51,
                AutoGenerateColumns = false
            };

            BlockTable = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 29,
                RowHeadersWidth = 51,
                AutoGenerateColumns = false,
                AllowUserToDeleteRows = true,
                AllowUserToAddRows = true,
            };

            CategoryTable.Columns.AddRange(new DataGridViewColumn[]
              {
                new DataGridViewTextBoxColumn
                {
                    Name = "Category",
                    HeaderText = "Category",
                    DataPropertyName = "Category",
                     ReadOnly = true
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Threat",
                    HeaderText = "Threat",
                    DataPropertyName = nameof(BlockCategoryThreat.Threat)
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Multiplier",
                    HeaderText = "Multiplier",
                    DataPropertyName = nameof(BlockCategoryThreat.Multiplier)
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Threshold",
                    HeaderText = "Threshold",
                    DataPropertyName = nameof(BlockCategoryThreat.MultiplierThreshold)
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "FullVolumeThreat",
                    HeaderText = "Full Volume Threat",
                    DataPropertyName = nameof(BlockCategoryThreat.FullVolumeThreat)
                }
              });

            BlockTable.Columns.AddRange(new DataGridViewColumn[]
                   {
                    new DataGridViewTextBoxColumn
                    {
                        Name = "Type",
                        HeaderText = "Type ID",
                        DataPropertyName = "BlockType",
                         ReadOnly = true
                    },
                    new DataGridViewTextBoxColumn
                    {
                        Name = "SubType",
                        HeaderText = "Subtype ID",
                        DataPropertyName = "BlockSubType",
                         ReadOnly = true
                    },
                    new DataGridViewTextBoxColumn
                    {
                        Name = "Threat",
                        HeaderText = "Threat",
                        DataPropertyName = "Threat"
                    },
                    new DataGridViewTextBoxColumn
                    {
                        Name = "Multiplier",
                        HeaderText = "Multiplier",
                        DataPropertyName = nameof(SingleBlockThreat.Multiplier)
                    },
                    new DataGridViewTextBoxColumn
                    {
                        Name = "Threshold",
                        HeaderText = "Threshold",
                        DataPropertyName = nameof(SingleBlockThreat.MultiplierThreshold)
                    },
                    new DataGridViewTextBoxColumn
                    {
                        Name = "FullVolumeThreat",
                        HeaderText = "Full Volume Threat",
                        DataPropertyName = nameof(SingleBlockThreat.FullVolumeThreat)
                    }
                   });
            #endregion


            CategoryTable.AllowUserToAddRows = true;
            BlockTable.AllowUserToAddRows = true;








            // -------- GridTypeTable --------
            GridTypeTable = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 29,
                RowHeadersWidth = 51,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false             
            };

            GridTypeTable.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "MultiplierType",
                    HeaderText = "Multiplier Type",
                    DataPropertyName = "Name",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "SmallGrid",
                    HeaderText = "Small Grid",
                    DataPropertyName = "SmallGridMultiplier",
                    ReadOnly = false
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "LargeGrid",
                    HeaderText = "Large Grid",
                    DataPropertyName = "LargeGridMultiplier",
                     ReadOnly = false
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Station",
                    HeaderText = "Station",
                    DataPropertyName = "StationMultiplier",
                     ReadOnly = false
                }
             });


            // Add tables to nested split panels
            rightTableSplitContainer.Panel1.Controls.Add(BlockTable);
            rightTableSplitContainer.Panel2.Controls.Add(CategoryTable);

            rightMainSplitContainer.Panel1.Controls.Add(rightTableSplitContainer);
            rightMainSplitContainer.Panel2.Controls.Add(GridTypeTable);
            // -------- Add controls to panels --------
            mainViewSplitContainer.Panel1.Controls.Add(xmlEditor);

            mainViewSplitContainer.Panel2.Controls.Add(rightMainSplitContainer);

            // Place SplitContainer inside ToolStripContainer
            mainContainer.ContentPanel.Controls.Add(mainViewSplitContainer);

            // -------- Form properties --------
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(mainContainer);

            Name = "Main View";
            Text = "Space Engineers Threat Config Editor";
            WindowState = FormWindowState.Maximized;

          
            CategoryTable.SelectionChanged += Table_SelectionChanged;
            CategoryTable.CellValueChanged += Table_CellValueChanged;

            BlockTable.CellValueChanged += Table_CellValueChanged;
            BlockTable.SelectionChanged += Table_SelectionChanged;

            GridTypeTable.SelectionChanged += GridTypeTable_SelectionChanged;
            GridTypeTable.CellValueChanged += GridTypeTable_CellValueChanged;
            ResumeLayout(false);
            ConfigureXmlLexer();
            ConfigureHighlighting();

            MESHelperState.Instance.MainFormLoaded(this);
        }

        private void XmlEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                MESHelperState.Instance.DocumentManager.SaveCurrentDocument();
            }
        }

        private void XmlEditor_LostFocus(object sender, EventArgs e)
        {
            State.DocumentManager.UpdateEditorUI();
        }

        private void GridTypeTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void GridTypeTable_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is DataGridView dgv && dgv.CurrentRow != null)
            {
                var rowIndex = dgv.CurrentRow.Index;
                var colIndex = dgv.CurrentCell.ColumnIndex;
                string multiplierName = dgv.CurrentRow.Cells[0].Value.ToString();

                if (multiplierName == null) return;
                string sectionName = colIndex switch
                {
                    1 => "SmallGridMultiplier",
                    2 => "LargeGridMultiplier",
                    3 => "StationMultiplier",
                    _ => null
                };
                int line = State.DocumentManager.GetMultiplierLine(multiplierName, sectionName );
                if (line < 0) return;

                var linePos = xmlEditor.Lines[line].Position;
                var lineEndPos = xmlEditor.Lines[line].EndPosition;

                xmlEditor.GotoPosition(linePos);
                xmlEditor.SetSelection(linePos, lineEndPos);
                xmlEditor.ScrollCaret();
            }
        }

        private Timer _updateTimer;
        private const int UpdateDelayMs = 500; // milliseconds delay

        private void XmlEditor_TextChanged(object sender, EventArgs e)
        {
        }

      

        private void Table_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is DataGridView)
            {
                var dgv = (DataGridView)sender;

                if (dgv.CurrentRow?.DataBoundItem is BlockCategoryThreat cat)
                {
                    int line = State.DocumentManager.GetCategoryLine(cat.Category);
                    int pos = xmlEditor.Lines[line].Position;
                    xmlEditor.GotoPosition(pos);
                    xmlEditor.SetSelection(xmlEditor.Lines[line].Position, xmlEditor.Lines[line].EndPosition);
                }
                else if (dgv.CurrentRow?.DataBoundItem is SingleBlockThreat blk)
                {
                    int line = State.DocumentManager.GetBlockLine(blk.BlockType, blk.BlockSubType);
                    int pos = xmlEditor.Lines[line].Position;
                    xmlEditor.GotoPosition(pos);
                    xmlEditor.SetSelection(xmlEditor.Lines[line].Position, xmlEditor.Lines[line].EndPosition);
                }
            }
        }
        private void Table_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
          
        }

        private string SerializeToXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false
            };

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty); // This removes xmlns:xsi and xmlns:xsd
            using var sw = new StringWriter();
            using (var writer = XmlWriter.Create(sw, settings))
            {
                serializer.Serialize(writer, obj, ns);
            }
            return sw.ToString();
        }

        
        private void XmlEditor_UpdateUI(object sender, UpdateUIEventArgs e)
        {
         
        }
        private const int HighlightIndicator = 8; // Choose an unused indicator index

        private void ConfigureHighlighting()
        {
            var indicator = xmlEditor.Indicators[HighlightIndicator];
            indicator.Style = IndicatorStyle.StraightBox;
            indicator.ForeColor = Color.LightYellow; // Background color
            indicator.Alpha = 180; // Transparency (0-255)
            indicator.Under = true;
        }

        private void HighlightLineBackground(int lineNumber)
        {
            if (lineNumber < 0 || lineNumber >= xmlEditor.Lines.Count)
                return;

            // Clear previous highlights for this indicator
            xmlEditor.IndicatorCurrent = HighlightIndicator;
            xmlEditor.IndicatorClearRange(0, xmlEditor.TextLength);

            var line = xmlEditor.Lines[lineNumber];
            xmlEditor.IndicatorFillRange(line.Position, line.Length);
        }

        private void HighlightMatchingRow(DataGridView table, string key)
        {
            foreach (DataGridViewRow row in table.Rows)
            {
                row.DefaultCellStyle.BackColor = table.DefaultCellStyle.BackColor;
            }

            foreach (DataGridViewRow row in table.Rows)
            {
                if (row.DataBoundItem is BlockCategoryThreat cat &&
                    string.Equals(cat.Category, key, StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    table.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
                else if (row.DataBoundItem is SingleBlockThreat blk &&
                         string.Equals(blk.BlockType, key, StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    table.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private void xmlEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void xmlEditor_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    var ext = Path.GetExtension(file).ToLowerInvariant();

                    try
                    {
                        if (ext == ".xml")
                        {
                            // Load into editor as normal
                            State.DocumentManager.LoadXml(file);
                        }
                        else if (ext == ".sbc")
                        {
                            State.ConfigManager.ProcessSbcFile(file);                           
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to process file '{file}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        #endregion


        

        private void OnOpenFile(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                State.DocumentManager.LoadXml(ofd.FileName);
            }
        }

        private void OnSaveFile(object sender, EventArgs e)
        {
            using SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, xmlEditor.Text);
            }
        } 
   
    }
}
