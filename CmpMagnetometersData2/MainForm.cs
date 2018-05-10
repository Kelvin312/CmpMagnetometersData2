using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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

            foreach (SequenceMeas item in clbFiles.CheckedItems)
            {
                var time = new DateTime(2018, 1, 1).Add(item.StartTime.TimeOfDay);
                var sd = new Series()
                {
                    ChartType = SeriesChartType.Line,
                    Color = item.ColorData,
                    XValueType = ChartValueType.DateTime,
                    // LabelFormat = "H:mm:ss",
                };
                var sm = new Series()
                {
                    ChartType = SeriesChartType.Line,
                    Color = item.ColorMsd,
                    XValueType = ChartValueType.DateTime,
                    // LabelFormat = "H:mm:ss",
                };

                var sub = item.Id == -1 ? 0 : (int) numSub.Value;

                foreach (var val in item.ItemList)
                {
                    if (item.IsViewData) sd.Points.AddXY(time.ToOADate(), val.Field - sub);
                    if (item.IsViewMsd) sm.Points.AddXY(time.ToOADate(), val.Qmc);
                    time = time.AddSeconds(item.StepTime);
                }

                if (item.IsViewData) chart.Series.Add(sd);
                if (item.IsViewMsd) chart.Series.Add(sm);
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


        private SequenceMeas currentSelected = null;

        private void NewSelected(SequenceMeas item)
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
                        SequenceMeas mg = new SequenceMeas();
                        mg.FileName = Path.GetFileNameWithoutExtension(file);
                        mg.IsViewData = true;
                        mg.IsViewMsd = true;
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

                                        bool is107 = mg.Id == 107;
                                        mg.ColorData = is107 ? Color.HotPink : Color.Green;
                                        mg.ColorMsd = is107 ? Color.LightPink : Color.LightGreen;

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
                                    ItemMeas v = new ItemMeas();
                                    v.Field = int.Parse(s[0]);
                                    v.Qmc = int.Parse(s[1]);
                                    v.State = (byte) int.Parse(s[2], NumberStyles.AllowHexSpecifier);
                                    var time = DateTime.ParseExact(s[3], "MM-dd-yy HH:mm:ss.ff", new CultureInfo("en-US"));

                                    mg.ItemList.Add(v);

                                    if (v.State != 0x80)
                                    {
                                        string description = "";
                                        if ((v.State & 0x40) != 0) description += " низкое напряжение питания (измерение не проводилось)";
                                        if ((v.State & 0x20) != 0) description += " нет сигнала (измерение не проводилось)";
                                        if ((v.State & 0x10) != 0) description += " результат не попадает в пределы 20000-100000 нTл";
                                        if ((v.State & 0x04) != 0) description += " низкое отношение сигнал/шум";
                                        if ((v.State & 0x02) != 0) description += " укорочение длительности сигнала";
                                        if ((v.State & 0x01) != 0) description += " значение поля не соответствует установленному рабочему поддиапазону";

                                        txtTimeBug.AppendText($"{i} {time} [{(int)v.State:X2}]{description}\r\n");
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

                        clbFiles.Items.Add(mg, true);
                    }
                }

              //  UpdateFileList();
            }
        }

        private void clbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewSelected((SequenceMeas)clbFiles.SelectedItem);
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
                StringBuilder sb = new StringBuilder(currentSelected.ItemList.Count*20);
                sb.AppendFormat("{0:D3}\0{1:D5}\0", currentSelected.Id, currentSelected.StepTime);
                DateTime t = currentSelected.StartTime;
                foreach (var val in currentSelected.ItemList)
                {
                    sb.AppendFormat("{0} +- {1:D5} [{2:X2}] {3:MM-dd-yy HH:mm:ss.ff}\0",val.Field,val.Qmc,(uint)val.State,t);
                    t = t.AddSeconds(currentSelected.StepTime);
                }
                using ( StreamWriter sw = new StreamWriter(sf.FileName))
                {
                    sw.Write(sb.ToString());
                }
            }
            
        }


        private double CorrelationExel(IEnumerable<double> fp, IEnumerable<double> sp)
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
                    sumAB += a * b;
                    sumAA += a * a;
                    sumBB += b * b;
                }
            var res = (n * sumAB - sumA * sumB) /
                      Math.Sqrt((n * sumAA - sumA * sumA) * (n * sumBB - sumB * sumB));
            return res;
        }

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
            var corrList = new List<SequenceMeas>();

            foreach (SequenceMeas item in clbFiles.CheckedItems)
            {
                if (item.IsViewData)
                {
                    corrList.Add(item);
                }
            }

            var lt = dtpStartCorTime.Value.TimeOfDay;
            var rt = dtpEndCorTime.Value.TimeOfDay;
            TimeSpan st;

            for (int fi = 0; fi < corrList.Count-1; fi++)
            {
                var aval = corrList[fi].GetTimeInterval(lt, rt, out st);
                if (aval != null)
                {
                    for (int si = fi + 1; si < corrList.Count; si++)
                    {
                        var bval = corrList[si].GetTimeInterval(lt, rt, out st);
                        if (bval != null)
                        {
                            var res = CorrelationExel(aval, bval);
                            txtTimeBug.AppendText(
                                $"C: {st} {corrList[fi]} и {corrList[si]} = {res}\r\n");

                            txtTimeBug.SelectionStart = txtTimeBug.Text.Length;
                            txtTimeBug.ScrollToCaret();
                        }
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtTimeBug.Text = "";
        }

        private void btnDelta_Click(object sender, EventArgs e)
        {
            var deltaList = new List<SequenceMeas>();
            int namei = 0;
            foreach (SequenceMeas a in clbFiles.CheckedItems)
            {
                if (a.Id != 107) continue;
                foreach (SequenceMeas b in clbFiles.CheckedItems)
                {
                    if (b.Id != 970) continue;
                    if (a.StartTime == b.StartTime && a.StepTime == b.StepTime)
                    {
                        deltaList.Add(new SequenceMeas()
                        {
                            FileName = $"Delta{namei}",
                            IsViewData = true,
                            IsViewMsd = false,
                            ColorData = Color.Blue,
                            ColorMsd = Color.AliceBlue,
                            Id = -1,
                            StartTime = a.StartTime,
                            StepTime = a.StepTime,
                            ItemList = a.ItemList.Zip(b.ItemList, (l, r) => new ItemMeas()
                                {
                                    Field = l.Field - r.Field,
                                    Qmc = l.Qmc - r.Qmc,
                                    State = (byte) (l.State | r.State),
                                })
                                .ToList(),
                        });
                        namei++;
                    }
                }
            }
            foreach (var delta in deltaList)
            {
                clbFiles.Items.Add(delta, true);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            currentSelected = null;
            clbFiles.Items.Remove(clbFiles.SelectedItem);
        }

        private void btnSaveExel_Click(object sender, EventArgs e)
        {

            var lt = dtpStartCorTime.Value.TimeOfDay;
            var rt = dtpEndCorTime.Value.TimeOfDay;
            TimeSpan st, curSt = lt;

            var data = new List<StringBuilder>();
            var sb = new StringBuilder();
            sb.AppendFormat("Time");


            foreach (SequenceMeas item in clbFiles.CheckedItems)
            {
                if (item.IsViewData)
                {
                    var aval = item.GetTimeInterval(lt, rt, out st);
                    if (aval != null)
                    {
                        curSt = st;
                        sb.AppendFormat($"\t{item.FileName}");
                        var i = 0;
                        foreach (var a in aval)
                        {
                            if(i>=data.Count) data.Add(new StringBuilder());
                            data[i].AppendFormat($"\t{a}");
                            i++;
                        }
                    }
                }
            }

            sb.AppendLine();
            foreach (var str in data)
            {
                sb.AppendFormat("{0}",curSt);
                curSt = curSt.Add(new TimeSpan( 0, 0, 3));
                sb.Append(str);
                sb.AppendLine();
            }

            SaveFileDialog sf = new SaveFileDialog() { DefaultExt = "txt", };
            if (sf.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sf.FileName))
                {
                    sw.Write(sb.ToString());
                }
            }
        }


        private void ChartUpdateAprox(int kpow)
        {
            var deleteIndex = new List<Series>();
            foreach (var ser in chart.Series)
            {
                if (ser.Tag != null && (int)ser.Tag == -1)
                {
                    deleteIndex.Add(ser);
                }
            }
            foreach (var ser in deleteIndex)
            {
                chart.Series.Remove(ser);
            }
            if (kpow >= 0)
            {
                var dataList = new List<IEnumerable<int>>();
                var lt = dtpStartCorTime.Value.TimeOfDay;
                var rt = dtpEndCorTime.Value.TimeOfDay;
                var sub = (int)numSub.Value;
                TimeSpan tst, st = new TimeSpan();
                int minn = int.MaxValue;
                foreach (SequenceMeas item in clbFiles.CheckedItems)
                {
                    if (item.IsViewData)
                    {
                        var t = item.GetTimeInterval(lt, rt, out tst);
                        if (t == null) continue;
                        st = tst;
                        dataList.Add(t);
                        minn = Math.Min(t.Count(), minn);
                    }
                }

                var abCorVal = new List<IEnumerable<double>>();

                foreach (var data in dataList)
                {
                    var time = new DateTime(2018, 1, 1).Add(st);
                    var sd = new Series()
                    {
                        ChartType = SeriesChartType.Line,
                        Color = Color.Black,
                        XValueType = ChartValueType.DateTime,
                        Tag = -1,
                        // LabelFormat = "H:mm:ss",
                    };

                    var p = Approximation.DataLine(Approximation.Conv(data, minn, kpow),minn,kpow);
                    abCorVal.Add(data.Zip(p,(fv,sv)=>fv-sv));

                    foreach (var val in p)
                    {
                        sd.Points.AddXY(time.ToOADate(), val-sub);
                        time = time.AddSeconds(3);
                    }
                    chart.Series.Add(sd);
                }

                if (abCorVal.Count > 1)
                {

                    var res = CorrelationExel(abCorVal[0], abCorVal[1]);
                    txtTimeBug.AppendText(
                        $"C: {kpow} | {st} = {res}\r\n");

                    txtTimeBug.SelectionStart = txtTimeBug.Text.Length;
                    txtTimeBug.ScrollToCaret();
                }
            }
            chart.Refresh();
        }


        private void btnUpdateAprox_Click(object sender, EventArgs e)
        {
            ChartUpdateAprox((int)numAproxPow.Value);
        }
    }
}
