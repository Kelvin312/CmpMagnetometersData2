namespace CmpMagnetometersData2
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.clbFiles = new System.Windows.Forms.CheckedListBox();
            this.cbData = new System.Windows.Forms.CheckBox();
            this.cbMSD = new System.Windows.Forms.CheckBox();
            this.dtpStartFile = new System.Windows.Forms.DateTimePicker();
            this.numStartFileMs = new System.Windows.Forms.NumericUpDown();
            this.btnChangeStart = new System.Windows.Forms.Button();
            this.txtTimeBug = new System.Windows.Forms.RichTextBox();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cmbColorData = new System.Windows.Forms.ComboBox();
            this.cmbColorMSD = new System.Windows.Forms.ComboBox();
            this.btnUpdChart = new System.Windows.Forms.Button();
            this.btnSaveToFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numStartFileMs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(1023, 361);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(1104, 361);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // clbFiles
            // 
            this.clbFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clbFiles.FormattingEnabled = true;
            this.clbFiles.Location = new System.Drawing.Point(1023, 12);
            this.clbFiles.Name = "clbFiles";
            this.clbFiles.Size = new System.Drawing.Size(156, 334);
            this.clbFiles.TabIndex = 2;
            this.clbFiles.SelectedIndexChanged += new System.EventHandler(this.clbFiles_SelectedIndexChanged);
            // 
            // cbData
            // 
            this.cbData.AutoSize = true;
            this.cbData.Location = new System.Drawing.Point(31, 337);
            this.cbData.Name = "cbData";
            this.cbData.Size = new System.Drawing.Size(61, 17);
            this.cbData.TabIndex = 3;
            this.cbData.Text = "cbData";
            this.cbData.UseVisualStyleBackColor = true;
            this.cbData.CheckedChanged += new System.EventHandler(this.ItemValueChange);
            // 
            // cbMSD
            // 
            this.cbMSD.AutoSize = true;
            this.cbMSD.Location = new System.Drawing.Point(31, 360);
            this.cbMSD.Name = "cbMSD";
            this.cbMSD.Size = new System.Drawing.Size(62, 17);
            this.cbMSD.TabIndex = 4;
            this.cbMSD.Text = "cbMSD";
            this.cbMSD.UseVisualStyleBackColor = true;
            this.cbMSD.CheckedChanged += new System.EventHandler(this.ItemValueChange);
            // 
            // dtpStartFile
            // 
            this.dtpStartFile.CustomFormat = "yyyy.MM.dd - HH:mm:ss";
            this.dtpStartFile.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartFile.Location = new System.Drawing.Point(269, 337);
            this.dtpStartFile.Name = "dtpStartFile";
            this.dtpStartFile.Size = new System.Drawing.Size(162, 20);
            this.dtpStartFile.TabIndex = 5;
            // 
            // numStartFileMs
            // 
            this.numStartFileMs.Location = new System.Drawing.Point(311, 361);
            this.numStartFileMs.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numStartFileMs.Name = "numStartFileMs";
            this.numStartFileMs.Size = new System.Drawing.Size(120, 20);
            this.numStartFileMs.TabIndex = 6;
            // 
            // btnChangeStart
            // 
            this.btnChangeStart.Location = new System.Drawing.Point(463, 361);
            this.btnChangeStart.Name = "btnChangeStart";
            this.btnChangeStart.Size = new System.Drawing.Size(75, 23);
            this.btnChangeStart.TabIndex = 7;
            this.btnChangeStart.Text = "btnChangeStart";
            this.btnChangeStart.UseVisualStyleBackColor = true;
            this.btnChangeStart.Click += new System.EventHandler(this.btnChangeStart_Click);
            // 
            // txtTimeBug
            // 
            this.txtTimeBug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTimeBug.Location = new System.Drawing.Point(713, 10);
            this.txtTimeBug.Name = "txtTimeBug";
            this.txtTimeBug.Size = new System.Drawing.Size(304, 336);
            this.txtTimeBug.TabIndex = 8;
            this.txtTimeBug.Text = "";
            // 
            // chart
            // 
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Location = new System.Drawing.Point(12, 10);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(695, 312);
            this.chart.TabIndex = 9;
            this.chart.Text = "chart";
            // 
            // cmbColorData
            // 
            this.cmbColorData.FormattingEnabled = true;
            this.cmbColorData.Location = new System.Drawing.Point(98, 337);
            this.cmbColorData.Name = "cmbColorData";
            this.cmbColorData.Size = new System.Drawing.Size(121, 21);
            this.cmbColorData.TabIndex = 10;
            this.cmbColorData.SelectedIndexChanged += new System.EventHandler(this.ItemValueChange);
            // 
            // cmbColorMSD
            // 
            this.cmbColorMSD.FormattingEnabled = true;
            this.cmbColorMSD.Location = new System.Drawing.Point(98, 364);
            this.cmbColorMSD.Name = "cmbColorMSD";
            this.cmbColorMSD.Size = new System.Drawing.Size(121, 21);
            this.cmbColorMSD.TabIndex = 11;
            this.cmbColorMSD.SelectedIndexChanged += new System.EventHandler(this.ItemValueChange);
            // 
            // btnUpdChart
            // 
            this.btnUpdChart.Location = new System.Drawing.Point(610, 361);
            this.btnUpdChart.Name = "btnUpdChart";
            this.btnUpdChart.Size = new System.Drawing.Size(75, 23);
            this.btnUpdChart.TabIndex = 12;
            this.btnUpdChart.Text = "Перерисовать";
            this.btnUpdChart.UseVisualStyleBackColor = true;
            this.btnUpdChart.Click += new System.EventHandler(this.btnUpdChart_Click);
            // 
            // btnSaveToFile
            // 
            this.btnSaveToFile.Location = new System.Drawing.Point(841, 361);
            this.btnSaveToFile.Name = "btnSaveToFile";
            this.btnSaveToFile.Size = new System.Drawing.Size(75, 23);
            this.btnSaveToFile.TabIndex = 13;
            this.btnSaveToFile.Text = "btnSaveToFile";
            this.btnSaveToFile.UseVisualStyleBackColor = true;
            this.btnSaveToFile.Click += new System.EventHandler(this.btnSaveToFile_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1191, 396);
            this.Controls.Add(this.btnSaveToFile);
            this.Controls.Add(this.btnUpdChart);
            this.Controls.Add(this.cmbColorMSD);
            this.Controls.Add(this.cmbColorData);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.txtTimeBug);
            this.Controls.Add(this.btnChangeStart);
            this.Controls.Add(this.numStartFileMs);
            this.Controls.Add(this.dtpStartFile);
            this.Controls.Add(this.cbMSD);
            this.Controls.Add(this.cbData);
            this.Controls.Add(this.clbFiles);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Name = "MainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numStartFileMs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.CheckedListBox clbFiles;
        private System.Windows.Forms.CheckBox cbData;
        private System.Windows.Forms.CheckBox cbMSD;
        private System.Windows.Forms.DateTimePicker dtpStartFile;
        private System.Windows.Forms.NumericUpDown numStartFileMs;
        private System.Windows.Forms.Button btnChangeStart;
        private System.Windows.Forms.RichTextBox txtTimeBug;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.ComboBox cmbColorData;
        private System.Windows.Forms.ComboBox cmbColorMSD;
        private System.Windows.Forms.Button btnUpdChart;
        private System.Windows.Forms.Button btnSaveToFile;
    }
}

