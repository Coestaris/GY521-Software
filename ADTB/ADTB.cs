using System;
using System.IO.Ports;
using System.Threading;

namespace ADTB
{
    public enum ConnectionStatus
    {
        UnableToOpenPort = 0x1,
        UnableToFindAnyPort = 0x2,
        OK = 0x4,
        UnableToInitDevice = 0x8,
        Timeout = 0x10
    }

    public enum TransferStatus
    {
        UnableToGetPacket = 0x1,
        UnEqualHashes = 0x2,
        OK = 0x4,
        TimeOut = 0x8,
    }

    public class ADTBClient
    {
        const int commandInit = 0x1;
        const int commandDataRequest = 0x2;
        const int commandInitAnsw = 0xA0;

        const int timeoutTries = 5;

        public int TimeoutDelay = 150;

        private SerialPort port;
        private bool dataRecieved;
        private byte[] bytesRecieved;
        
        public ConnectionStatus Connect(string portName)
        {
            port = new SerialPort(portName, 115200);
            try
            {
                port.Open();
                port.ReceivedBytesThreshold = 1;
                port.DataReceived += Port_DataReceived;
                for (int n = 0; n < timeoutTries; n++)
                {
                    dataRecieved = false;
                    bytesRecieved = new byte[1];
                    port.Write(new byte[] { commandInit }, 0, 1);
                    int timeoutDelay = TimeoutDelay;
                    while (timeoutDelay > 0)
                    {
                        if (dataRecieved)
                        {
                            if (bytesRecieved.Length != 1)
                            {
                                port.Close();
                                return ConnectionStatus.UnableToInitDevice;
                            }
                            if (bytesRecieved[0] != commandInitAnsw)
                            {
                                port.Close();
                                return ConnectionStatus.UnableToInitDevice;
                            }

                            port.ReceivedBytesThreshold = 20;
                            return ConnectionStatus.OK;
                        }
                        timeoutDelay--;
                        Thread.Sleep(1);
                    }
                }
                port.Close();
                return ConnectionStatus.Timeout;
            }
            catch(Exception)
            {
                return ConnectionStatus.UnableToOpenPort;
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                for (int i = 0; i < bytesRecieved.Length; i++)
                    lock (port) bytesRecieved[i] = (byte)port.ReadByte();
            } catch { return; }
            dataRecieved = true;
            port.DiscardInBuffer();
        }

        public void Disconnect()
        {
            if (port != null && port.IsOpen)
                port.Close();
        }

        public ADTBRawPacket GetNext()
        {
            try
            {
                if (port.IsOpen)
                {
                    for (int n = 0; n < timeoutTries; n++)
                    {
                        port.Write(new byte[] { commandDataRequest }, 0, 1);
                        int timeoutDelay = TimeoutDelay;
                        bytesRecieved = new byte[20];
                        dataRecieved = false;
                        while (timeoutDelay > 0)
                        {
                            if (dataRecieved)
                                return new ADTBRawPacket(bytesRecieved);
                            timeoutDelay--;
                            Thread.Sleep(1);
                        }
                    }
                    return new ADTBRawPacket(TransferStatus.TimeOut);
                }
                else return new ADTBRawPacket(TransferStatus.UnableToGetPacket);
            } catch(Exception)
            {
                return new ADTBRawPacket(TransferStatus.UnableToGetPacket);
            }
        }
    }
}
