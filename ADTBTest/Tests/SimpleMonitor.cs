using ADTB;
using System;
using System.Threading;

namespace ADTBTest
{
    public class SimpleMonitor : TestClass
    {
        public override void Run(object Delay)
        {
            Init();

            int delay = int.Parse((string)Delay);

            Console.WriteLine("Нажмите Esc для выхода. . .\n");

            while (true)
            {
                Thread.Sleep(delay);
                var packet = arduino.GetNext();
                if (packet.Status != TransferStatus.OK)
                {
                    Console.Write("Ошибка получения пакета: {0}", packet.Status);
                    Error();
                }
                else Console.WriteLine(packet);

                if (Console.KeyAvailable)
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        arduino.Disconnect();
                        return;
                    }
            }
        }
    }
}
