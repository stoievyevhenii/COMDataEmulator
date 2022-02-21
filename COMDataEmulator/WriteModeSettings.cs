using System.IO.Ports;

namespace COMDataEmulator
{
    public class WriteModeSettings : IWriteModeSettings
    {
        public string ComPort { get; set; }
        public int SendTimeout { get; set; }
        public string Data { get; set; }
        public string Mode { get; set; }
        public int COMSpeed { get; set; }
        public int Length { get; set; }
        public StopBits StopBit { get; set; }
        public Parity Parity { get; set; }
    }
}
