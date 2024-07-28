using NAudio.Wave;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace audiokaptur
{
    public class AudioCapture
    {
        private WasapiLoopbackCapture capture;

        public event EventHandler<WaveInEventArgs> OnDataAvailable;

        public AudioCapture()
        {
            capture = new WasapiLoopbackCapture();
            capture.DataAvailable += Capture_DataAvailable;
        }

        private void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            OnDataAvailable?.Invoke(this, e);
        }

        public void Start()
        {
            capture.StartRecording();
        }

        public void Stop()
        {
            capture.StopRecording();
        }
    }
}
