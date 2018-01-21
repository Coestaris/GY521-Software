using ADTB;
using System;

namespace ADTBTest
{
    public abstract class TestClass
    {
        protected const string portName = "COM18";

        protected ADTBClient arduino;

        protected void Init()
        {
            arduino = new ADTBClient
            {
                TimeoutDelay = 150 //150ms
            };
            Console.Write("Попытка подключится к {0}. Статус: ", portName);
            var status = arduino.Connect(portName);
            Console.WriteLine(status);

            if (status != ConnectionStatus.OK)
                Error();
        }

        protected static void Error()
        {
            Console.WriteLine("Нажмите любую клавишу чтобы продолжить. . .");
            Console.ReadKey();
            Environment.Exit(1);
        }

        public abstract void Run(object data);
    }
}
