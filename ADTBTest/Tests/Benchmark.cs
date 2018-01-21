using ADTB;
using System;

namespace ADTBTest
{
    public class Benchmark : TestClass
    {
        public override void Run(object testsCount)
        {
            Init();

            int count = int.Parse((string)testsCount);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int badReq = 0;
            for (int i = 0; i < count; i++)
            {
                var packet = arduino.GetNext();
                if (packet.Status != TransferStatus.OK)
                    badReq++;
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Выполнено {0} запросов за {1} мс. Запросов с ошибками: {3}. {2} запр/сек",
                count, elapsedMs, (float)count * 1000 / elapsedMs, badReq);

            arduino.Disconnect();
        }
    }
}
