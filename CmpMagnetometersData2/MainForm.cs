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

            AxisY.ScaleView.MinSize = 10;

            AxisX.IsStartedFromZero = false;
            AxisY.IsStartedFromZero = false;

            AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;

            //  chart.Series[0].IsXValueIndexed = true;
            chart.MouseDown += OnChartMouseDown;
            chart.MouseUp += OnChartMouseUp;

            //chart1.Series[0].XValueType = ChartValueType.Int32;

            
               AxisX.LabelStyle.Format = "H:mm:ss";

            // chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
             //chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            //chart.ChartAreas[0].CursorX.IntervalType = DateTimeIntervalType.Milliseconds;
            chart.ChartAreas[0].CursorX.Interval = 0;
            chart.ChartAreas[0].CursorY.Interval = 0;
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

            clbFiles.DisplayMember = "FileName";
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

       

        private void AxisXZoom(double a, double b)
        {
            AxisX.ScaleView.Zoom(a,b);
            ChangeAxisX();
        }
        private void AxisYZoom(double a, double b)
        {
            AxisY.ScaleView.Zoom((int)a, (int)b);
        }

        private void AxisXScroll(double a)
        {
            AxisX.ScaleView.Scroll(a);
            ChangeAxisX();
        }                
        private void AxisYScroll(double a)
        {
            AxisY.ScaleView.Scroll((int)a);
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
                    AxisXZoom(x, x + dx);
                    AxisYZoom(y, y + dy);

                }
                else if (mousedx < 0 && mousedy < 0)
                {
                    var szx = AxisX.ScaleView.ViewMaximum - AxisX.ScaleView.ViewMinimum;
                    var szy = AxisY.ScaleView.ViewMaximum - AxisY.ScaleView.ViewMinimum;
                    dx = szx * szx / -dx;
                    dy = szy * szy / -dy;
                    x = AxisX.ScaleView.Position + szx / 2 - dx / 2;
                    y = AxisY.ScaleView.Position + szy / 2 - dy / 2;
                    AxisXZoom(x, x + dx);
                    AxisYZoom(y, y + dy);
                }
                else
                {
                    AxisX.ScaleView.ZoomReset();
                    AxisY.ScaleView.ZoomReset();
                    ChangeAxisX();
                }
            }
            if (SelectButton == MouseButtons.Right)
            {
                AxisXScroll(AxisX.ScaleView.Position - dx);
                AxisYScroll(AxisY.ScaleView.Position + dy);
            }
            SelectButton = MouseButtons.None;
        }


        #endregion



        private void ChangeAxisX()
        {
           dtpStartCorTime.Value =  DateTime.FromOADate(AxisX.ScaleView.ViewMinimum);
          dtpEndCorTime.Value =   DateTime.FromOADate(AxisX.ScaleView.ViewMaximum);

        }

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

            chart.ResetAutoValues();
            chart.Refresh();
        }

        private void UpdateFileList()
        {
         //  clbFiles.DataSource = MultiDatas;
         //  //clbFiles.DataSource = (MultiDatas.Count > 0) ? new BindingSource(MultiDatas, null) : null;
         //  clbFiles.DisplayMember = "FileName";
         //  //clbFiles.ValueMember = "FileName";
         //  clbFiles.Refresh();
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


     //   public List<MagData> MultiDatas = new List<MagData>();


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
                        mg.IsViewData = true;
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
                                    v.Status = (byte) int.Parse(s[2], NumberStyles.AllowHexSpecifier);
                                    var time = DateTime.ParseExact(s[3], "MM-dd-yy HH:mm:ss.ff", new CultureInfo("en-US"));

                                    mg.ValList.Add(v);

                                    if (v.Status != 0x80)
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

                        clbFiles.Items.Add(mg);
                    }
                }

              //  UpdateFileList();
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
            if (sf.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder(currentSelected.ValList.Count*20);
                sb.AppendFormat("{0:D3}\0{1:D5}\0", currentSelected.Id, currentSelected.StepTime);
                DateTime t = currentSelected.StartTime;
                foreach (var val in currentSelected.ValList)
                {
                    sb.AppendFormat("{0} +- {1:D5} [{2:X2}] {3:MM-dd-yy HH:mm:ss.ff}\0",val.Data,val.Msd,(uint)val.Status,t);
                    t = t.AddSeconds(currentSelected.StepTime);
                }
                using ( StreamWriter sw = new StreamWriter(sf.FileName))
                {
                    sw.Write(sb.ToString());
                }
            }
            
        }


        delegate void MultiSum(int a, int b);

        private double CorrelationExel(IEnumerable<int> fp, IEnumerable<int> sp)
        {
            double sumA = 0, sumB = 0, sumAB = 0, sumAA = 0, sumBB = 0;
            var n = Math.Min(fp.Count(), sp.Count());
            using (var se = sp.GetEnumerator())
                foreach (var a in fp)
                {
                    if (!se.MoveNext()) break;
                    var b = se.Current;
                    sumA += a;
                    sumB += b;
                    sumAB += a * 1L * b;
                    sumAA += a * 1L * a;
                    sumBB += b * 1L * b;
                }
            var res = (n * sumAB - sumA * sumB) /
                         Math.Sqrt((n * sumAA - sumA * sumA) * (n * sumBB - sumB * sumB));
            return res;
        }

        private void btnCorrelation_Click(object sender, EventArgs e)
        {
            var corrList = new List<MagData>();

            foreach (MagData item in clbFiles.CheckedItems)
            {
                if (item.IsViewData)
                {
                    corrList.Add(item);
                }
            }

            var lt = dtpStartCorTime.Value.TimeOfDay;
            var rt = dtpEndCorTime.Value.TimeOfDay;

            for (int fi = 0; fi < corrList.Count-1; fi++)
            {
                var aval = corrList[fi].GetTimeInterval(lt, rt);
                if (aval != null)
                {
                    for (int si = fi + 1; si < corrList.Count; si++)
                    {
                        var bval = corrList[si].GetTimeInterval(lt, rt);
                        if (bval != null)
                        {
                            var res = CorrelationExel(aval, bval);
                            txtTimeBug.AppendText(
                                $"C: {corrList[fi]} и {corrList[si]} = {res}\r\n");
                        }
                    }
                }
            }
        }
        //txtValues.AppendText(
            //    _chartForms[fi].GetFileName()
            //    + " и "
            //    + _chartForms[si].GetFileName()
            //    + " = "
            //    + res.ToString("F")
            //    + "\r\n\r\n");

        
    }
}
