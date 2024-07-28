using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace audiokaptur
{
    public class FFT
    {
        public static void ComputeFFT(Complex[] data)
        {
            int n = data.Length;
            if (n <= 1) return;

            var even = new Complex[n / 2];
            var odd = new Complex[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                even[i] = data[i * 2];
                odd[i] = data[i * 2 + 1];
            }

            ComputeFFT(even);
            ComputeFFT(odd);

            for (int k = 0; k < n / 2; k++)
            {
                var t = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * k / n) * odd[k];
                data[k] = even[k] + t;
                data[k + n / 2] = even[k] - t;
            }
        }
    }
}
