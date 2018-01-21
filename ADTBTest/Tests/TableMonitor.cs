using ADTB;
using System;
using System.Threading;

namespace ADTBTest
{
    public class TableMonitor : TestClass
    {
        ADTBSmoothResultStack smoothResultStack;

        private const int smoothLevel = 30;

        public override void Run(object Delay)
        {
            Init();

            int delay = int.Parse((string)Delay);
            smoothResultStack = new ADTBSmoothResultStack(smoothLevel);

            Console.WriteLine("Нажмите Esc для выхода. . .\n");
            int defPos = Console.CursorTop;

            while (true)
            {
                Thread.Sleep(delay);
                var packet = arduino.GetNext();
                if (packet.Status != TransferStatus.OK)
                {
                    Console.Write("Ошибка получения пакета: {0}", packet.Status);
                    Error();
                }
                else
                {
                    smoothResultStack.Push(packet.Normalize(ADTBRawPacket.AngleFormat.Degrees, ADTBRawPacket.TempFormat.Celsius));
                    var data = smoothResultStack.Current();

                    Console.WriteLine("_Pitch:{0,10:0.000}°", data.Pitch);
                    Console.WriteLine("  Roll:{0,10:0.000}°", data.Roll);
                    //Console.WriteLine("_AcZ:{0,10:0.0}°", data.AcZ);
                    Console.WriteLine();
                    Console.WriteLine("_GyX:{0,10:0.00}", data.GyX);
                    Console.WriteLine("_GyY:{0,10:0.00}", data.GyY);
                    Console.WriteLine("_GyZ:{0,10:0.00}", data.GyZ);
                    Console.WriteLine();
                    Console.WriteLine("_Tmp:{0,10:0.0}°C", data.Tmp);
                    Console.SetCursorPosition(0, defPos);

                    if(Console.KeyAvailable)
                        if(Console.ReadKey().Key == ConsoleKey.Escape)
                        {
                            arduino.Disconnect();
                            Console.SetCursorPosition(0, defPos + 10);
                            return;
                        }
                }
            }
        }
    }
}
