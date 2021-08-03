using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ThreadSquareEquation
{
    class Program
    {
        static async Task Main()
        {
            var stopWatch = new Stopwatch();
            FileWrite2 fileWrite = new FileWrite2(1000);

            stopWatch.Start();
            await fileWrite.Write();
            stopWatch.Stop();

            Console.WriteLine("______________________________________________");
            Console.WriteLine(value: $" seconds {stopWatch.Elapsed.Seconds}  ms {stopWatch.Elapsed.Milliseconds}");
            Console.WriteLine("______________________________________________");

        }
    }
}
