using NAudio.Wave;
using Pastel;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace audiokaptur
{
    class Program
    {
        static void Main(string[] args)
        {
            AudioCapture audioCapture = new AudioCapture();
            audioCapture.OnDataAvailable += AudioCapture_OnDataAvailable;

            int opt = 0;

            start:
            audioCapture.Start();
            Console.ReadKey();
            audioCapture.Stop();
            Console.Clear();
            Console.WriteLine(" SETTINGS ");

            Console.WriteLine(" => visualiser ".Pastel(Color.Plum));
            Console.WriteLine("    colors ");

            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key) 
            {
                case ConsoleKey.UpArrow:
                    opt++;
                    break;
                case ConsoleKey.DownArrow:
                    opt--; 
                    break;
                case ConsoleKey.Enter:
                    break;
                case ConsoleKey.Escape:
                    goto start;
            }


/*            string option = Console.ReadLine();
            if(option == "vis") 
            {
                goto start;
            }*/
            
        }

        private static void AudioCapture_OnDataAvailable(object sender, WaveInEventArgs e)
        {
            const int fftLength = 4096;
            Complex[] fftBuffer = new Complex[fftLength];

            for (int i = 0; i < fftLength && i < e.BytesRecorded / 2; i++)
            {
                short sample = BitConverter.ToInt16(e.Buffer, i * 2);
                fftBuffer[i] = new Complex(sample, 0);
            }

            FFT.ComputeFFT(fftBuffer);

            var magnitudes = fftBuffer.Take(fftLength / 2)
                .Select(c => c.Magnitude)
                .ToArray();

            DisplaySpectrum(magnitudes, 32, 16000, 24);
        }

        private static void DisplaySpectrum(double[] magnitudes, double startFrequency, double endFrequency, int bands)
        {
            Console.OutputEncoding = Encoding.UTF8;

            double factor = Math.Pow(endFrequency / startFrequency, 1.0 / (bands - 1));


            double[] frequencyBands = new double[bands];

            for (int i = 0; i < bands; i++)
            {
                frequencyBands[i] = startFrequency * Math.Pow(factor, i);
            }

            int samplesPerBand = magnitudes.Length / bands;

            for (int i = 0; i < bands; i++)
            {
                double avg = magnitudes.Skip(i * samplesPerBand).Take(samplesPerBand).Average();
                int roundedFrequency = (int)Math.Round(frequencyBands[i]);

                List<string> lines = new List<string>();

                for (int j = 0; j < (int)Math.Ceiling(avg / 90000); j++)
                {
                    lines.Add(getStrColored(j, 2)); 
                }

                Console.WriteLine($"" +
                    $"{roundedFrequency} Hz: " +
                    $"{new string(' ', 5 - roundedFrequency.ToString().Length)} " +
                    $"{string.Join("", lines)}" +
                    $"{(int)Math.Ceiling(avg / 100000)}" +
                    $"{new string(' ', 20)}"
                    );

                lines.Clear();
            }

            Console.SetCursorPosition(0, 0);
        }

        static string getStrColored(int avg, int mode)
        {
            if(mode == 1)
            {
                return avg switch
                {
                    >= 0 and <= 5 => "█".Pastel(Color.Green),
                    > 5 and <= 10 => "█".Pastel(Color.Yellow),
                    > 10 => "█".Pastel(Color.Red),
                    _ => "█"
                };
            }
            else
            {
                int red = (int)Math.Abs(((avg - 1) * 255 / 20)) > 255 ? 255 : (int)Math.Abs(((avg - 1) * 255 / 20));
                return "█".Pastel(Color.FromArgb(red, 130, 40)); // gradient green -> red

            }
        }
    }
}
