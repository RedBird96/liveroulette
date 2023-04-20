
namespace LiveRoulette
{
    partial class ScenarioTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScenarioTest));
            this.comboScenario = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstOutput = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lstView_Stats = new System.Windows.Forms.ListView();
            this.colStatsNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatsNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatsOddEven = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatsDozen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatsColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatsHalf = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstInput = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // comboScenario
            // 
            this.comboScenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboScenario.FormattingEnabled = true;
            this.comboScenario.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q"});
            this.comboScenario.Location = new System.Drawing.Point(154, 12);
            this.comboScenario.Name = "comboScenario";
            this.comboScenario.Size = new System.Drawing.Size(80, 21);
            this.comboScenario.TabIndex = 1;
            this.comboScenario.SelectedIndexChanged += new System.EventHandler(this.comboScenario_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Algorithm List :";
            // 
            // lstOutput
            // 
            this.lstOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstOutput.FormattingEnabled = true;
            this.lstOutput.Location = new System.Drawing.Point(12, 363);
            this.lstOutput.Name = "lstOutput";
            this.lstOutput.Size = new System.Drawing.Size(287, 225);
            this.lstOutput.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Input";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 345);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Output";
            // 
            // lstView_Stats
            // 
            this.lstView_Stats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstView_Stats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colStatsNo,
            this.colStatsNumber,
            this.colStatsOddEven,
            this.colStatsDozen,
            this.colStatsColumn,
            this.colStatsHalf});
            this.lstView_Stats.FullRowSelect = true;
            this.lstView_Stats.HideSelection = false;
            this.lstView_Stats.Location = new System.Drawing.Point(320, 46);
            this.lstView_Stats.Name = "lstView_Stats";
            this.lstView_Stats.Size = new System.Drawing.Size(502, 542);
            this.lstView_Stats.TabIndex = 10;
            this.lstView_Stats.UseCompatibleStateImageBehavior = false;
            this.lstView_Stats.View = System.Windows.Forms.View.Details;
            // 
            // colStatsNo
            // 
            this.colStatsNo.Text = "No";
            this.colStatsNo.Width = 30;
            // 
            // colStatsNumber
            // 
            this.colStatsNumber.Text = "Number";
            this.colStatsNumber.Width = 55;
            // 
            // colStatsOddEven
            // 
            this.colStatsOddEven.Text = "Odd/Even";
            this.colStatsOddEven.Width = 64;
            // 
            // colStatsDozen
            // 
            this.colStatsDozen.Text = "Dozen";
            this.colStatsDozen.Width = 51;
            // 
            // colStatsColumn
            // 
            this.colStatsColumn.Text = "2to1";
            this.colStatsColumn.Width = 62;
            // 
            // colStatsHalf
            // 
            this.colStatsHalf.Text = "1-18/19-36";
            this.colStatsHalf.Width = 92;
            // 
            // lstInput
            // 
            this.lstInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstInput.FormattingEnabled = true;
            this.lstInput.HorizontalScrollbar = true;
            this.lstInput.Location = new System.Drawing.Point(12, 46);
            this.lstInput.Name = "lstInput";
            this.lstInput.Size = new System.Drawing.Size(287, 290);
            this.lstInput.TabIndex = 11;
            this.lstInput.SelectedIndexChanged += new System.EventHandler(this.lstInput_SelectedIndexChanged);
            // 
            // ScenarioTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 605);
            this.Controls.Add(this.lstInput);
            this.Controls.Add(this.lstView_Stats);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstOutput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboScenario);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScenarioTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ScenarioTest";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScenarioTest_FormClosed);
            this.Load += new System.EventHandler(this.ScenarioTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboScenario;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView lstView_Stats;
        private System.Windows.Forms.ColumnHeader colStatsNo;
        private System.Windows.Forms.ColumnHeader colStatsNumber;
        private System.Windows.Forms.ColumnHeader colStatsOddEven;
        private System.Windows.Forms.ColumnHeader colStatsDozen;
        private System.Windows.Forms.ColumnHeader colStatsColumn;
        private System.Windows.Forms.ColumnHeader colStatsHalf;
        private System.Windows.Forms.ListBox lstInput;
    }
}