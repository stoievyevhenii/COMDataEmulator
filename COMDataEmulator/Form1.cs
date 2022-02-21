using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
            InitStopBits();
            InitParity();
        }

        #region INITS
        private void InitSendDataMode()
        {
            try
            {
                var modes = new[] { Mode.ASCII, Mode.Byte };
                foreach (var mode in modes)
                {
                    modeComboBox.Items.Add(mode);
                }

                modeComboBox.SelectedIndex = 0;
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

                if (portsArray.Length <= 0)
                {
                    COMPortsComboBox.Enabled = false;
                    button1.Enabled = false;
                }
                else
                {
                    foreach (var port in portsArray)
                    {
                        COMPortsComboBox.Items.Add(port);
                    }

                    COMPortsComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                statusField.Text = e.Message;
            }
        }
        private void InitStopBits()
        {
            try
            {
                var stopBitsList = Enum.GetValues(typeof(StopBits)).Cast<StopBits>();

                foreach (var stopBit in stopBitsList)
                {
                    stopBitComboBox.Items.Add(stopBit.ToString());
                }

                stopBitComboBox.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                statusField.Text = ex.Message;
            }
        }
        private void InitParity()
        {
            try
            {
                var parityList = Enum.GetValues(typeof(Parity)).Cast<Parity>();

                foreach (var parity in parityList)
                {
                    parityCombBox.Items.Add(parity.ToString());
                }

                parityCombBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                statusField.Text = ex.Message;
            }
        }
        #endregion

        #region ACIONS
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var stopBitFlag = Enum.TryParse(stopBitComboBox.SelectedItem.ToString(), out StopBits stopBits);
                var parityFlag = Enum.TryParse(parityCombBox.SelectedItem.ToString(), out Parity parity);

                var writeSettings = new WriteModeSettings()
                {
                    ComPort = COMPortsComboBox.SelectedItem.ToString(),
                    SendTimeout = int.Parse(COMSendTimeout.Text),
                    Data = DataForSend.Text,
                    Mode = modeComboBox.SelectedItem.ToString(),
                    COMSpeed = int.Parse(comPortSpeed.Text),
                    Length = int.Parse(COMDataBits.Text),
                    StopBit = stopBits,
                    Parity = parity,
                };

                Thread thred = new Thread(() => StartDataSend(writeSettings));
                thred.Start();

                button1.Enabled = false;
                button2.Enabled = true;
            }
            catch (Exception ex)
            {
                statusField.Text = ex.Message;
            }
        }
        private void StartDataSend(WriteModeSettings settings)
        {
            SerialPort _serialPort = new SerialPort(
                                settings.ComPort,
                                settings.COMSpeed,
                                settings.Parity,
                                settings.Length,
                                settings.StopBit);

            _serialPort.Handshake = Handshake.None;
            _serialPort.WriteTimeout = settings.SendTimeout;

            while (activeSend)
            {
                if (!(_serialPort.IsOpen))
                {
                    _serialPort.Open();

                    if (settings.Mode == Mode.ASCII)
                    {
                        _serialPort.Write(settings.Data);
                    }
                    else
                    {
                        var byteString = Encoding.ASCII.GetBytes(settings.Data);
                        _serialPort.Write(byteString, 0, 8);
                    }

                    _serialPort.Write(settings.Data);
                };
            }

            _serialPort.Close();
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
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (string.Equals((sender as Button).Name, @"CloseButton"))
            {
                StopSend();
            }
        }
        #endregion
    }
}
