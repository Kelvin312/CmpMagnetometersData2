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
    }
}
