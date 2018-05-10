using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmpMagnetometersData2
{
    public class Approximation
    {
        public static double[] DataLine(int n, int k, double[] polynom)
        {
            var res = new double[n];
            Array.Clear(res, 0, res.Length);
            for (int i = 1; i <= n; i++)
            {
                double px = 1;
                for (int j = 0; j <= k; j++)
                {
                    res[i - 1] += px * polynom[j];
                    px *= i;
                }
            }
            return res;
        }
        public static double[] Conv(IEnumerable<int> arg)
        {
            const int maxk = 12;
            var sumx = new double[maxk * 2 + 1];
            var a = new double[maxk+1, maxk+1+1];
            var b = new double[maxk + 1];
            Array.Clear(sumx, 0, sumx.Length);
            Array.Clear(b, 0, b.Length);
            int n = arg.Count();
            var y = arg.GetEnumerator();
            for (int i = 1; i <= n; i++)
            {
                y.MoveNext();
                double px = 1;
                for (int k = 0; k <= 2*maxk; k++)
                {
                    sumx[k] += px;
                    if (k <= maxk) b[k] += px * y.Current;
                    px *= i;
                }
            }
            for (int i = 0; i <= maxk; i++)
            {
                for (int j = 0; j <= maxk; j++)
                {
                    a[i, j] = sumx[i + j];
                }
                a[i, maxk + 1] = b[i];
            }
            for (int i = 0; i < maxk; i++)
            {
                for (int j = i+1; j <= maxk; j++)
                {
                    double m = a[j, i] / a[i, i];
                    for (int k = i; k <=maxk+1; k++)
                    {
                        a[j, k] -= a[i, k] * m;
                    }
                }
            }
            for (int i = 0; i <= maxk; i++)
            {
                double m = a[maxk - i, maxk + 1];
                for (int j = 0; j < i; j++)
                {
                    m -= a[maxk - i, maxk - j] * b[maxk - j];
                }
                m /= a[maxk - i, maxk - i];
                b[maxk - i] = m;
            }
            return b;
        }
    }
}
