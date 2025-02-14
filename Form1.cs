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
            }
            LoadAudioDevices();
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
            this.FormClosing += Form1_FormClosing;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private void LoadAudioDevices()
        {
            var devices = CSCore.CoreAudioAPI.MMDeviceEnumerator.EnumerateDevices(
              CSCore.CoreAudioAPI.DataFlow.Capture,
           CSCore.CoreAudioAPI.DeviceState.Active);
    
            Console.WriteLine("🔍 Scanning Input Devices...");
            foreach (var device in devices)
            {
                Console.WriteLine($"🎤 Found Input Device: {device.FriendlyName}");
                comboBoxInputDevices.Items.Add(device.FriendlyName);
            }

            devices = CSCore.CoreAudioAPI.MMDeviceEnumerator.EnumerateDevices(
                CSCore.CoreAudioAPI.DataFlow.Render,
                CSCore.CoreAudioAPI.DeviceState.Active);

            Console.WriteLine("🔍 Scanning Output Devices...");
            foreach (var device in devices)
            {
                Console.WriteLine($"🔊 Found Output Device: {device.FriendlyName}");
                comboBoxOutputDevices.Items.Add(device.FriendlyName);
            }

            if (comboBoxInputDevices.Items.Count > 0) comboBoxInputDevices.SelectedIndex = 0;
            if (comboBoxOutputDevices.Items.Count > 0) comboBoxOutputDevices.SelectedIndex = 0;

            
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
             if (comboBoxInputDevices.SelectedIndex == -1 || comboBoxOutputDevices.SelectedIndex == -1)
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
                     .ChangeSampleRate(48000)
                     .ToStereo();

                 effectStream = new EffectStream(source);

                 var outputDevices = CSCore.CoreAudioAPI.MMDeviceEnumerator.EnumerateDevices(
                     CSCore.CoreAudioAPI.DataFlow.Render,
                     CSCore.CoreAudioAPI.DeviceState.Active);

                 var selectedOutputDevice = outputDevices[comboBoxOutputDevices.SelectedIndex];

                soundOut = new CSCore.SoundOut.WasapiOut();
                soundOut.Device = selectedOutputDevice;
                soundOut.Latency = 41;
                soundOut.Initialize(effectStream.ToWaveSource(16));

                MessageBox.Show($"Using Input Device: {selectedInputDevice.FriendlyName}");
                 MessageBox.Show($"Using Output Device: {selectedOutputDevice.FriendlyName}");

                 capture.Start();
                 MessageBox.Show("🎤 Capture started!");

                 soundOut.Play();
                 MessageBox.Show("🔊 Output started!");
             }
             catch (Exception ex)
             {
                 MessageBox.Show("Error: " + ex.Message);
             }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
               // MessageBox.Show("🛑 Stopping audio...");

                // Stop and dispose of the output
                if (soundOut != null)
                {
                    if (soundOut.PlaybackState == CSCore.SoundOut.PlaybackState.Playing)
                    {
                        soundOut.Stop();
                    }
                    soundOut.Dispose();
                    soundOut = null;
                }

                // Stop and dispose of the capture
                if (capture != null)
                {
                    capture.Stop();
                    capture.Dispose();
                    capture = null;
                }

                // Dispose of effect stream
                if (effectStream != null)
                {
                    effectStream.Dispose();
                    effectStream = null;
                }

                if (source != null)
                {
                    source = null;
                }

                Console.WriteLine("✅ Audio fully stopped!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error stopping audio: " + ex.Message);
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("🔴 App is closing, stopping audio...");
            buttonStop_Click(sender, e);  // Call stop method before exit
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }




}
