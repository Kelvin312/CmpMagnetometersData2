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
        public static double[] DataLine(double[] polynom, int n, int kpow)
        {
            var res = new double[n];
            Array.Clear(res, 0, res.Length);
            for (int i = 1; i <= n; i++)
            {
                double px = 1;
                for (int j = 0; j <= kpow; j++)
                {
                    res[i - 1] += px * polynom[j];
                    px *= i;
                }
            }
            return res;
        }
        public static double[] Conv(IEnumerable<int> arg, int n, int kpow)
        {
            var sumx = new double[kpow * 2 + 1];
            var a = new double[kpow+1, kpow+1+1];
            var b = new double[kpow + 1];
            Array.Clear(sumx, 0, sumx.Length);
            Array.Clear(b, 0, b.Length);
            using (var y = arg.GetEnumerator())
                for (int i = 1; i <= n; i++)
                {
                    y.MoveNext();
                    double px = 1;
                    for (int k = 0; k <= 2 * kpow; k++)
                    {
                        sumx[k] += px;
                        if (k <= kpow) b[k] += px * y.Current;
                        px *= i;
                    }
                }
            for (int i = 0; i <= kpow; i++)
            {
                for (int j = 0; j <= kpow; j++)
                {
                    a[i, j] = sumx[i + j];
                }
                a[i, kpow + 1] = b[i];
            }
            for (int i = 0; i < kpow; i++)
            {
                for (int j = i+1; j <= kpow; j++)
                {
                    double m = a[j, i] / a[i, i];
                    for (int k = i; k <=kpow+1; k++)
                    {
                        a[j, k] -= a[i, k] * m;
                    }
                }
            }
            for (int i = 0; i <= kpow; i++)
            {
                double m = a[kpow - i, kpow + 1];
                for (int j = 0; j < i; j++)
                {
                    m -= a[kpow - i, kpow - j] * b[kpow - j];
                }
                m /= a[kpow - i, kpow - i];
                b[kpow - i] = m;
            }
            return b;
        }
    }
}
