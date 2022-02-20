using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace COMDataEmulator
{
    public partial class Form1 : Form
    {
        static bool activeSend = true;

        public Form1()
        {
            InitializeComponent();
            InitComPorts();
            InitSendDataMode();
        }

        private void InitSendDataMode()
        {
            try
            {
                var modes = new[] { "ASCII", "byte", "HEX" };
                foreach (var mode in modes)
                {
                    modeComboBox.Items.Add(mode);
                }
            }
            catch (Exception e)
            {
                statusField.Text = e.Message;
            }

            statusField.Text = "";
        }
        private void InitComPorts()
        {
            try
            {
                var portsArray = SerialPort.GetPortNames();

                foreach (var port in portsArray)
                {
                    COMPortsComboBox.Items.Add(port);
                }
            }
            catch (Exception e)
            {
                statusField.Text = e.Message;
            }
        }

        // Form elements actions

        private void button1_Click(object sender, EventArgs e)
        {
            var enteredText = DataForSend.Text;
            var comPort = COMPortsComboBox.SelectedItem.ToString();
            var sendTimeout = int.Parse(COMSendTimeout.Text);

            Thread thred = new Thread(() => StartDataSend(comPort, sendTimeout, enteredText));
            thred.Start();

            button1.Enabled = false;
            button2.Enabled = true;
        }
        private void StartDataSend(string comPort, int sendTimeout, string data)
        {
            try
            {
                SerialPort _serialPort = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
                _serialPort.Handshake = Handshake.None;
                _serialPort.WriteTimeout = sendTimeout;

                while (activeSend)
                {
                    try
                    {
                        if (!(_serialPort.IsOpen))
                        {
                            _serialPort.Open();
                            _serialPort.Write(data);
                        };
                    }
                    catch (Exception e)
                    {
                        statusField.Text = e.Message;
                    }
                }

                _serialPort.Close();
            }
            catch (Exception e)
            {
                statusField.Text = e.Message;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StopSend();
        }

        private void StopSend()
        {
            activeSend = false;

            button1.Enabled = true;
            button2.Enabled = false;
        }
    }
}
