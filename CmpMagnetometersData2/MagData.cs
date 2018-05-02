using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CmpMagnetometersData2
{
    public struct ItemMeas
    {
        public int Field { get; set; }
        public int Qmc { get; set; }
        public byte State { get; set; }
        public bool IsTimeTravel { get; set; }
    }
    public class SequenceMeas
    {
        public string FileName;
        public int Id;
        public int StepTime;
        public DateTime StartTime;
        public DateTime EndTime;
        public List<ItemMeas> ItemList;

        public override string ToString()
        {
            return FileName;
        }

       public SequenceMeas() {ItemList = new List<ItemMeas>(); }

        public SequenceMeas(string fileText, string fileName, ref StringBuilder parseLog)
        {
            var culture = CultureInfo.CreateSpecificCulture("ja-JP");

            FileName = fileName;
            var splitText = fileText.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
            Id = int.Parse(splitText[0]);
            StepTime = int.Parse(splitText[1]);
            ItemList = new List<ItemMeas>(splitText.Length - 2);

            var currentTime = new DateTime();
            var isInitTime = true;
            for (int i = 2; i < splitText.Length; i++)
            {
                try
                {
                    var val = splitText[i].Split(new[] {" +- ", " [", "] "}, StringSplitOptions.RemoveEmptyEntries);
                    var time = DateTime.ParseExact(val[3], "MM-dd-yy HH:mm:ss.ff", culture);
                    if (isInitTime)
                    {
                        currentTime = time;
                        isInitTime = false;
                    }

                    var item = new ItemMeas()
                    {
                        Field = int.Parse(val[0]),
                        Qmc = int.Parse(val[1]),
                        State = byte.Parse(val[2], NumberStyles.AllowHexSpecifier),
                        IsTimeTravel = time != currentTime,
                    };

                    ItemList.Add(item);


                    if (item.State != 0x80)
                    {
                        parseLog.AppendLine($"{i} {currentTime.ToString(culture)} [{item.State:X2}]");
                        if (item.State == 0x7F) parseLog.AppendLine("сбой в программе");
                        else
                        {
                            if ((item.State & 0x40) != 0) parseLog.AppendLine("- низкое напряжение питания (измерение не проводилось)");
                            if ((item.State & 0x20) != 0) parseLog.AppendLine("- нет сигнала (измерение не проводилось)");
                            if ((item.State & 0x10) != 0) parseLog.AppendLine("- результат не попадает в пределы 20000-100000 нTл");
                            if ((item.State & 0x04) != 0) parseLog.AppendLine("- низкое отношение сигнал/шум");
                            if ((item.State & 0x02) != 0) parseLog.AppendLine("- укорочение длительности сигнала");
                            if ((item.State & 0x01) != 0) parseLog.AppendLine("- значение поля не соответствует установленному рабочему поддиапазону");
                        }
                    }
                    if (item.IsTimeTravel)
                    {
                        if (item.State == 0x80) parseLog.AppendLine($"{i} {currentTime.ToString(culture)}");
                        parseLog.AppendLine($"путешествие во времени {time.ToString(culture)}");
                    }
                }
                catch (Exception)
                {
                    parseLog.AppendLine($"{i} {currentTime.ToString(culture)}");
                    parseLog.AppendLine(splitText[i]);
                }
                currentTime = currentTime.AddSeconds(StepTime);
            }


        }
        //  public SequenceMeas() { }

        

        
        public Color ColorData;
        public Color ColorMsd;
        public bool IsViewData;
        public bool IsViewMsd;
        

        public IEnumerable<int> GetTimeInterval(TimeSpan l, TimeSpan r, out TimeSpan st)
        {
            var start = StartTime.TimeOfDay;
            st = start;
            if ( start  <= l && start <= r && l<=r)
            {
                var li = (int)Math.Round((l - start).TotalSeconds / StepTime);
                if (li < ItemList.Count)
                {
                    st = st.Add(new TimeSpan(0, 0, li * StepTime));
                    var ri = (int) Math.Round((r - start).TotalSeconds / StepTime);
                    if (ri >= ItemList.Count) ri = ItemList.Count-1;
                    return ItemList.Skip(li).Take(ri - li + 1).Select(x => x.Field);
                }
            }
            return null;
        }
    }
}
