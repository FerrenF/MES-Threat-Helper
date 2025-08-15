using MESHelper.Threat;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static MESHelper.BlueprintParser;

namespace MESHelper
{

    public partial class ViewThreatProfile : Form
    {

        public DataGridView ThreatConfigProfileGrid;
        private Panel dropPanel;
        private Label dropLabel;

        public MESHelperState? CurrentState = MESHelperState.Instance;

        public ViewThreatProfile()
        {           
            InitializeComponents();           
            if(CurrentState != null)
            {
                CurrentState.ViewThreatProfileViewInstance = this;
                CurrentState.ProfileViewLoaded(this);
            }                     
        }

        private void InitializeComponents()
        {
            this.Text = "Threat Configuration Comparison";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeDropPanel();
            InitializeThreatConfigGrid();
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            dropPanel.DragEnter += BlueprintDropPanelDragEnter;
            dropPanel.DragDrop += BlueprintDropPanelDragDrop;
        }
        private void InitializeDropPanel()
        {
            dropPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                AllowDrop = true,
                BackColor = System.Drawing.Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
            };

            dropLabel = new Label
            {
                Text = "Drop Space Engineers blueprint files (.sbc) here",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
            };

            dropPanel.Controls.Add(dropLabel);
            this.Controls.Add(dropPanel);
        }

        private void InitializeThreatConfigGrid()
        {
            ThreatConfigProfileGrid = new DataGridView
            {
                Dock = DockStyle.Top,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Height = 300

            };

            // Columns for EntityThreatProfile
            ThreatConfigProfileGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DisplayName",
                HeaderText = "Name",
                Width = 200
            });
            ThreatConfigProfileGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GridType",
                HeaderText = "Grid Size",
                Width = 100
            });
            ThreatConfigProfileGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NumBlocks",
                HeaderText = "# Blocks",
                Width = 75
            });
            ThreatConfigProfileGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Threat",
                HeaderText = "Threat",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            
            this.Controls.Add(ThreatConfigProfileGrid);
        }

        private void BlueprintDropPanelDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Any(f => IsBlueprintFile(f)))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
            else         
                e.Effect = DragDropEffects.None;
        }

        private void BlueprintDropPanelDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var blueprintFiles = files.Where(f => IsBlueprintFile(f)).ToList();

            if (blueprintFiles.Count == 0)
            {
                MessageBox.Show("No valid Space Engineers blueprint files detected.","Invalid files", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var file in blueprintFiles)
            {
                CurrentState.ThreatProfileManager?.ImportConfigurationFromFile(file);          
            }
        }
                
        

        private bool IsBlueprintFile(string filePath)
        {
            // Adjust extensions as appropriate for Space Engineers blueprints
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext == ".sbc" || ext == ".xml";
        }
    }
}
