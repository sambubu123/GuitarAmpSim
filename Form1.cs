using System;
using System.Windows.Forms;
using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace GuitarAmpSim
{
    public partial class Form1 : Form
    {
        private CSCore.SoundIn.WasapiCapture capture;  // Explicitly CSCore
        private ISampleSource source;                 // Processes audio input
        private CSCore.SoundOut.WasapiOut soundOut;   // Explicitly CSCore
        private EffectStream effectStream;            // Custom effects processor

        public Form1()
        {
            InitializeComponent();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                AllocConsole();
                Console.WriteLine("Console initialized!");
            }
            Console.WriteLine("wusgud my nigga");
            LoadAudioDevices();
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private void LoadAudioDevices()
        {
            var enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator();

            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                comboBoxInputDevices.Items.Add(device.FriendlyName);
            }

            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                comboBoxOutputDevices.Items.Add(device.FriendlyName);
            }

            if (comboBoxInputDevices.Items.Count > 0) comboBoxInputDevices.SelectedIndex = 0;
            if (comboBoxOutputDevices.Items.Count > 0) comboBoxOutputDevices.SelectedIndex = 0;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
           /* if (comboBoxInputDevices.SelectedIndex == -1 || comboBoxOutputDevices.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an input and output device.");
                return;
            }

            try
            {
                var inputDevices = CSCore.CoreAudioAPI.MMDeviceEnumerator.EnumerateDevices(
                    CSCore.CoreAudioAPI.DataFlow.Capture,
                    CSCore.CoreAudioAPI.DeviceState.Active);

                var selectedInputDevice = inputDevices[comboBoxInputDevices.SelectedIndex];

                capture = new CSCore.SoundIn.WasapiCapture();  // Explicitly CSCore
                capture.Device = selectedInputDevice;
                capture.Initialize();

                source = new SoundInSource(capture) { FillWithZeros = false }
                    .ToSampleSource()
                    .ChangeSampleRate(44100)
                    .ToStereo();

                effectStream = new EffectStream(source);

                var outputDevices = CSCore.CoreAudioAPI.MMDeviceEnumerator.EnumerateDevices(
                    CSCore.CoreAudioAPI.DataFlow.Render,
                    CSCore.CoreAudioAPI.DeviceState.Active);

                var selectedOutputDevice = outputDevices[comboBoxOutputDevices.SelectedIndex];

                soundOut = new CSCore.SoundOut.WasapiOut();  // Explicitly CSCore
                soundOut.Device = selectedOutputDevice;
                soundOut.Initialize(effectStream.ToWaveSource(16));

                Console.WriteLine($"Using Input Device: {selectedInputDevice.FriendlyName}");
                Console.WriteLine($"Using Output Device: {selectedOutputDevice.FriendlyName}");

                capture.Start();
                Console.WriteLine("🎤 Capture started!");

                soundOut.Play();
                Console.WriteLine("🔊 Output started!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }*/
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            soundOut?.Stop();
            capture?.Stop();
            soundOut?.Dispose();
            capture?.Dispose();
        }

       
    }

  

  
}
