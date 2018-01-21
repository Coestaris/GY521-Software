using System;
using System.Linq;

namespace ADTBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Выберите необходимую опцию и укажите параметр команды: ");

                var type = typeof(TestClass);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p))
                    .ToList()
                    .FindAll(p => p != type)
                    .ToArray();

                int i = 0;
                foreach (var item in types) Console.WriteLine($"{i++}) {item.Name}");

                var input = Console.ReadLine().Trim().Split(' ');
                var index = int.Parse(input[0]);

                Console.WriteLine();
                Console.WriteLine($"Выполнение класса {types[index].FullName} {(input.Length != 1 ? "с параметром " + input[1] : "")}");
                Console.WriteLine();

                var obj = (TestClass)Activator.CreateInstance(types[index]);
                if (input.Length == 1) obj.Run("0");
                else obj.Run(input[1]);

                Console.WriteLine();
                Console.WriteLine("=================");
                Console.WriteLine();
            }
        }
    }
}
