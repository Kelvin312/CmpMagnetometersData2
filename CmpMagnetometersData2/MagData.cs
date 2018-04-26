using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmpMagnetometersData2
{
    public struct MagVal
    {
        public int Data { get; set; }
        public int Msd { get; set; }
        public byte Status { get; set; }
    }
    public class MagData
    {
        public List<MagVal> ValList = new List<MagVal>();
        public int Id;
        public int StepTime;
        public string FileName;
        public DateTime StartTime;
        public Color ColorData;
        public Color ColorMsd;
        public bool IsViewData;
        public bool IsViewMsd;
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return FileName;
        }

        public IEnumerable<int> GetTimeInterval(TimeSpan l, TimeSpan r)
        {
            var start = StartTime.TimeOfDay;
            if ( start  <= l && start <= r && l<r)
            {
                var li = (int)Math.Round((l - start).TotalSeconds / StepTime);
                if (li < ValList.Count)
                {
                    var ri = (int) Math.Round((r - start).TotalSeconds / StepTime);
                    if (ri >= ValList.Count) ri = ValList.Count-1;
                    return ValList.Skip(li).Take(ri - li + 1).Select(x => x.Data);
                }
            }
            return null;
        }
    }
}
