using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CmpMagnetometersData2
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            SelectButton = MouseButtons.None;

            AxisX = chart.ChartAreas[0].AxisX;
            AxisY = chart.ChartAreas[0].AxisY;
           // AxisX.LabelStyle.Format = "F0";
            // AxisY.LabelStyle.Format = "F0";
            AxisX.ScrollBar.Enabled = false;
            AxisY.ScrollBar.Enabled = false;

            //  chart.Series[0].IsXValueIndexed = true;
            chart.MouseDown += OnChartMouseDown;
            chart.MouseUp += OnChartMouseUp;

            //chart1.Series[0].XValueType = ChartValueType.Int32;

            
               AxisX.LabelStyle.Format = "H:mm:ss";

             chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
             chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            chart.ChartAreas[0].CursorX.IntervalType = DateTimeIntervalType.Milliseconds;
            chart.ChartAreas[0].CursorX.Interval = 1;
            // chart.ChartAreas[0].CursorX.SelectionColor = Color.Transparent;
            // chart.ChartAreas[0].CursorY.SelectionColor = Color.Transparent;
            //chart.ChartAreas[0].CursorX.LineDashStyle = ChartDashStyle.Dash;


            Type colorType = typeof(Color);
            // We take only static property to avoid properties like Name, IsSystemColor ...
            var propInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public).Select(x=>Color.FromName(x.Name)).ToList();

            cmbColorData.DataSource = new BindingSource(propInfos, null);
            cmbColorData.DisplayMember = "Name";
            cmbColorMSD.DataSource = new BindingSource(propInfos, null);
            cmbColorMSD.DisplayMember = "Name";
        }

        #region ChartMouse

        

        

        private Axis AxisX { get; set; }
        private Axis AxisY { get; set; }
        private Point StartSelect { get; set; }
        private MouseButtons SelectButton { get; set; }


        private void OnChartMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                SelectButton = e.Button;
                StartSelect = new Point(e.X, e.Y);
            }
        }

        private void OnChartMouseUp(object sender, MouseEventArgs e)
        {
            if (SelectButton == MouseButtons.None) return;

            HitTestResult htr = chart.HitTest(e.X, e.Y);
            if (htr.ChartElementType == ChartElementType.Nothing) return;

            var mousedx = e.X - StartSelect.X;
            var mousedy = e.Y - StartSelect.Y;
            var x = AxisX.PixelPositionToValue(StartSelect.X);
            var y = AxisY.PixelPositionToValue(e.Y);
            var dx = AxisX.PixelPositionToValue(e.X) - x;
            var dy = AxisY.PixelPositionToValue(StartSelect.Y) - y;

            if (SelectButton == MouseButtons.Left)
            {
                if (mousedx > 0 && mousedy > 0)
                {
                    AxisX.ScaleView.Zoom(x, x + dx);
                    AxisY.ScaleView.Zoom(y, y + dy);

                }
                else if (mousedx < 0 && mousedy < 0)
                {
                    var szx = AxisX.ScaleView.ViewMaximum - AxisX.ScaleView.ViewMinimum;
                    var szy = AxisY.ScaleView.ViewMaximum - AxisY.ScaleView.ViewMinimum;
                    dx = szx * szx / -dx;
                    dy = szy * szy / -dy;
                    x = AxisX.ScaleView.Position + szx / 2 - dx / 2;
                    y = AxisY.ScaleView.Position + szy / 2 - dy / 2;
                    AxisX.ScaleView.Zoom(x, x + dx);
                    AxisY.ScaleView.Zoom(y, y + dy);
                }
                else
                {
                    AxisX.ScaleView.ZoomReset();
                    AxisY.ScaleView.ZoomReset();
                }
            }
            if (SelectButton == MouseButtons.Right)
            {
                AxisX.ScaleView.Scroll(AxisX.ScaleView.Position - dx);
                AxisY.ScaleView.Scroll(AxisY.ScaleView.Position + dy);
            }
            SelectButton = MouseButtons.None;
        }


        #endregion



        private void UpdateChart()
        {
            chart.Series.Clear();
            
            foreach (MagData item in clbFiles.CheckedItems)
            {
                if (item.IsViewData)
                {
                    DateTime time = item.StartTime;
                    Series s = new Series() {
                        ChartType = SeriesChartType.Line,
                        Color = item.ColorData,
                        XValueType = ChartValueType.DateTime,
                       // LabelFormat = "H:mm:ss",
                    };
                    foreach (var val in item.ValList)
                    {
                        s.Points.AddXY(time.ToOADate(), val.Data);
                        time = time.AddSeconds(item.StepTime);
                    }
                    
                    chart.Series.Add(s);
                }
            }
        }

        private void UpdateFileList()
        {
            clbFiles.DataSource = MultiDatas;
            //clbFiles.DataSource = (MultiDatas.Count > 0) ? new BindingSource(MultiDatas, null) : null;
            clbFiles.DisplayMember = "FileName";
            //clbFiles.ValueMember = "FileName";
            clbFiles.Refresh();
        }


        private MagData currentSelected = null;

        private void NewSelected(MagData item)
        {
            currentSelected = item;
            cmbColorData.SelectedItem = item.ColorData;
            cmbColorMSD.SelectedItem = item.ColorMsd;
            cbData.Checked = item.IsViewData;
            cbMSD.Checked = item.IsViewMsd;
            dtpStartFile.Value = item.StartTime;
        }


        public List<MagData> MultiDatas = new List<MagData>();


        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = true,
                // InitialDirectory = Environment.CurrentDirectory,
                Title = "Файлы магнитки",
                DefaultExt = "txt",
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in fileDialog.FileNames)
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        int i = -2;
                        MagData mg = new MagData();
                        mg.FileName = Path.GetFileNameWithoutExtension(file);
                        mg.ColorData = Color.Green;
                        mg.ColorMsd = Color.Green;
                        mg.IsViewData = false;
                        mg.IsViewMsd = false;
                        DateTime countTime = DateTime.Now;
                        txtTimeBug.AppendText($"file: {mg.FileName}\r\n");


                        foreach (var val in sr.ReadToEnd().Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (i < 0)
                            {
                                switch (i)
                                {
                                    case -2:
                                        mg.Id = int.Parse(val);
                                        break;
                                    case -1:
                                        mg.StepTime = int.Parse(val);
                                        break;
                                }

                            }
                            else
                            {
                                try
                                {
                                    var s = val.Split(new[] {" +- ", " [", "] "}, StringSplitOptions.RemoveEmptyEntries);
                                    MagVal v = new MagVal();
                                    v.Data = int.Parse(s[0]);
                                    v.Msd = int.Parse(s[1]);
                                    v.Status = (byte) int.Parse(s[2]);
                                    var time = DateTime.ParseExact(s[3], "MM-dd-yy HH:mm:ss.ff", new CultureInfo("en-US"));

                                    mg.ValList.Add(v);

                                    if (v.Status != 80)
                                    {
                                        txtTimeBug.AppendText($"{i} [{(int)v.Status}] {time}\r\n");
                                    }

                                    if (i > 0)
                                    {
                                        if (countTime.CompareTo(time) != 0)
                                        {
                                            txtTimeBug.AppendText($"{i} {countTime} != {time}\r\n");
                                        }
                                    }

                                    if (i == 0)
                                    {
                                        mg.StartTime = time;
                                        countTime = time;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    txtTimeBug.AppendText($"{i} Err: {val}\r\n");
                                    //throw;
                                }
                                countTime = countTime.AddSeconds(mg.StepTime);
                            }

                            i++;
                        }

                        MultiDatas.Add(mg);
                    }
                }

                UpdateFileList();
            }
        }

        private void clbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewSelected((MagData)clbFiles.SelectedItem);
        }

        private void ItemValueChange(object sender, EventArgs e)
        {
            if (!((Control) sender).Focused || currentSelected == null) return;

            currentSelected.ColorData = (Color) cmbColorData.SelectedItem;
            currentSelected.ColorMsd = (Color) cmbColorMSD.SelectedItem;
            currentSelected.IsViewData = cbData.Checked;
            currentSelected.IsViewMsd = cbMSD.Checked;
            
        }

        private void btnChangeStart_Click(object sender, EventArgs e)
        {
            if (!((Control)sender).Focused || currentSelected == null) return;
            currentSelected.StartTime = dtpStartFile.Value;
        }

        private void btnUpdChart_Click(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            if (currentSelected == null) return;
            SaveFileDialog sf = new SaveFileDialog() { DefaultExt = "txt", };
            
        }
    }
}
