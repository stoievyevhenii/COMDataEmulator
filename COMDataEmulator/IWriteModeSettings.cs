using System.IO.Ports;

namespace COMDataEmulator
{
    public interface IWriteModeSettings
    {
        string ComPort { get; set; }
        int SendTimeout { get; set; }
        string Data { get; set; }
        string Mode { get; set; }
        int COMSpeed { get; set; }
        int Length { get; set; }
        StopBits StopBit { get; set; }
        Parity Parity { get; set; }
    }
}