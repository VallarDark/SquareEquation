namespace ThreadSquareEquation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using EquationSolver;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    class FileWrite
    {
        private readonly string _fileForReadPath;
        private readonly string _fileForWritePath1;
        private readonly string _fileForWritePath2;
        private readonly List<Equation> _equations;
        private readonly Stopwatch _stopWatch;
        private readonly int _count;

        private bool _isCreated;


        public List<Equation> Equations => _equations.ToList();

        public FileWrite(int count)
        {
            var projectPath = Directory.GetCurrentDirectory();
            _fileForReadPath = projectPath + @"\FileForRead.txt";
            _fileForWritePath1 = projectPath + @"\FileForWrite1.txt";
            _fileForWritePath2 = projectPath + @"\FileForWrite2.txt";
            _equations = new List<Equation>();
            _stopWatch = new Stopwatch();
            _count = count;
        }


        public async Task CreateFileForRead()
        {
            await using StreamWriter sw = new StreamWriter(path: _fileForReadPath, append: false, encoding: System.Text.Encoding.Default);
            for (int i = 0; i < _count; i++)
            {

                var random = new Random();
                var a = random.Next(-11, 11);
                random = new Random();
                var b = random.Next(-11, 11);
                random = new Random();
                var c = random.Next(-11, 11);
                await sw.WriteLineAsync($"{a} {b} {c}");

            }
        }

        public async Task<List<Equation>> Read()
        {
            var result = new List<Equation>();
            if (!_isCreated)
            {
                await CreateFileForRead();
                _isCreated = true;
            }

            using StreamReader sr = new StreamReader(_fileForReadPath);
            while (!sr.EndOfStream)
            {
                await Task.Run(() =>
                {
                    var eqLine = sr.ReadLineAsync().Result;

                    if (eqLine != null)
                    {
                        var eqStrings = eqLine.Split();
                        if (double.TryParse(eqStrings[0], out double aResult) &&
                            double.TryParse(eqStrings[1], out double bResult) &&
                            double.TryParse(eqStrings[2], out double cResult))
                        {
                            var eq = new Equation() { A = aResult, B = bResult, C = cResult };
                            _equations.Add(eq);
                            result.Add(eq);
                        }
                    }
                });
            }
            return result;
        }

        public async Task Write()
        {
            _equations.Clear();

            EqSolver solver = new EqSolver();

            await using StreamWriter sw = new StreamWriter(_fileForWritePath2, false, System.Text.Encoding.Default);
            _stopWatch.Reset();
            _stopWatch.Start();

            try
            {
                List<Task> tasks=new List<Task>();
                var collection = await Read();
                foreach (var equation in collection)
                {

                    tasks.Add(Task.Run(async () =>
                    {
                        var eqSolution = solver.ResolveEquation(equation).Result.Explanation;

                        await TryWrite(sw, eqSolution);
                        Console.WriteLine("W___");
                        Log(eqSolution);
                        //sw.WriteLineAsync(eqSolution);

                    }));
                }

                Task.WaitAll(tasks.ToArray());
                _stopWatch.Stop();

                Console.WriteLine("______________________________________________");
                Console.WriteLine(value: $" seconds {_stopWatch.Elapsed.Seconds}  ms {_stopWatch.Elapsed.Milliseconds}");
                Console.WriteLine("______________________________________________");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SolveAndWrite()
        {
            _equations.Clear();

            if (!_isCreated)
            {
                await CreateFileForRead();
                _isCreated = true;
            }

            EqSolver solver = new EqSolver();

            await using StreamWriter sw = new StreamWriter(_fileForWritePath1, false, System.Text.Encoding.Default);
            using StreamReader sr = new StreamReader(_fileForReadPath);
            _stopWatch.Reset();
            _stopWatch.Start();
            try
            {
                
                while (!sr.EndOfStream)
                {
                    var task=Task.Run( async () =>
                    {
                        var eqLine = TryRead(sr).Result;

                        if (eqLine != null)
                        {
                            var eqStrings = eqLine.Split();
                            if (double.TryParse(eqStrings[0], out double aResult) &&
                                double.TryParse(eqStrings[1], out double bResult) &&
                                double.TryParse(eqStrings[2], out double cResult))
                            {
                                var eq = new Equation() { A = aResult, B = bResult, C = cResult };

                                _equations.Add(eq);
                                var result = solver.ResolveEquation(eq).Result.Explanation;
                                Console.WriteLine("SandW");
                                Log(result);
                                await TryWrite(sw, result);
                            }
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            _stopWatch.Stop();
            Console.WriteLine("______________________________________________");
            Console.WriteLine(value: $" seconds {_stopWatch.Elapsed.Seconds}  ms {_stopWatch.Elapsed.Milliseconds}");
            Console.WriteLine("______________________________________________");

        }



        private async Task TryWrite(StreamWriter writer, string data,int count=0)
        {
            if (count>10)
            {
                return;
            }
            try
            {
                writer?.WriteLineAsync(data);
            }
            catch
            {
                await TryWrite(writer, data, ++count);
            }
        }

        private async Task<string> TryRead(StreamReader reader,int count= 0)
        {
            if (count > 10)
            {
                return null;
            }
            try
            {
                return reader.ReadLineAsync().Result;
            }
            catch
            {
                return await TryRead(reader, ++count);
            }
        }


        private void Log(string solution)
        {

            Console.WriteLine(solution);
        }
    }
}
