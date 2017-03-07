using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewtonInterpolation.Tools
{
    static class Tools
    {
        static double[] BNewton(int n, double[] x, double[] y)
        {
            double[] b = new double[n];
            double num, denom;

            b[0] = y[0];
            for (int i = 1; i < n; i++)
            {
                num = y[i] - b[0];
                denom = x[i] - x[0];
                for (int j = 1; j < i; j++)
                {
                    double c = 1;
                    for (int k = 0; k < j; k++)
                        c *= x[i] - x[k];

                    num -= b[j] * c;
                    denom *= x[i] - x[j];
                }
                b[i] = num / denom;
            }

            return b;
        }

        static Dictionary<int, double> SkobkiNewton(double[] x, int n)
        {
            var res = new Dictionary<int, double>();
            res.Add(0, 1);

            for (int i = 0; i < n; i++)
            {
                Dictionary<int, double> temp = new Dictionary<int, double>();
                for (int j = 0; j < res.Count; j++)
                {
                    if (temp.ContainsKey(j))
                        temp[j] += res[j] * -x[i];
                    else
                        temp.Add(j, res[j] * -x[i]);
                    if (temp.ContainsKey(j + 1))
                        temp[j + 1] += res[j];
                    else
                        temp.Add(j + 1, res[j]);
                }

                foreach (var c in temp)
                    res[c.Key] = c.Value;
            }

            return res;
        }

        public static Dictionary<int, double> Newton(int n, double[] x, double[] y)
        {
            var res = new Dictionary<int, double>();
            var b = new double[6];

            b = BNewton(n, x, y);
            res.Add(0, b[0]);
            
            for (int i = 1; i < n; i++)
            {

                var tmp = SkobkiNewton(x, i);
                foreach (var c in tmp)
                {
                    if (res.ContainsKey(c.Key))
                        res[c.Key] += c.Value * b[i];
                    else
                        res.Add(c.Key, c.Value * b[i]);
                }
            }

            return res;
        }
    }
}
