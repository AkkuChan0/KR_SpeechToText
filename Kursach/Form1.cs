using NAudio.Wave;
using Vosk;


namespace Kursach
{
    public partial class Form1 : Form
    {
        bool isVoice = false;
        static WaveFileWriter? writer;

        Model model;
        VoskRecognizer rec;
        WaveInEvent waveIn;

        public Form1()
        {
            InitializeComponent();
            label1.Text = "Запись не идёт.";

            model = new Model("vosk-model");
            rec = new VoskRecognizer(model, 16000f);
        }

        private void WaveInOnDataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);

                if (rec.AcceptWaveform(e.Buffer, e.BytesRecorded))
                {
                    var temp = rec.Result();
                    var a = temp.Substring(0, temp.Length - 3);
                    a = a.Substring(14);
                    Invoke(new Action(() => { resultLabel.Text += "\n" + a; }));
                }
            }
            catch { }
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            if (isVoice)
            {
                isVoice = false;
                label1.Text = "Запись не идёт.";
                startStopButton.Text = "Пуск";

                waveIn?.StopRecording();
                writer?.Close();
                return;
            }

            isVoice = true;
            label1.Text = "Идёт запись...";
            startStopButton.Text = "Стоп";

            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(16000, 1);

            waveIn.DataAvailable += WaveInOnDataAvailable;

            writer = new WaveFileWriter("test.wav", waveIn.WaveFormat);

            waveIn.StartRecording();
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Text files|*.txt|All Files|*.*";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    File.WriteAllText(dialog.FileName, resultLabel.Text);
            }
        }

        private void clearButton_Click(object sender, EventArgs e) 
        { 
            resultLabel.Text = ""; 
        }

        private void Form1_Load(object sender, EventArgs e) {}
    }
}