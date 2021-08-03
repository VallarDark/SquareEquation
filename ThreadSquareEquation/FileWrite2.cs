namespace ThreadSquareEquation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using EquationSolver;
    using System.Linq;
    using System.Threading.Tasks;

    class FileWrite2
    {
        private readonly string _fileForReadPath;
        private readonly string _fileForWritePath;
        private readonly int _count;
        private bool _isCreated;

        public FileWrite2(int count)
        {
            var projectPath = Directory.GetCurrentDirectory();
            _fileForReadPath = projectPath + @"\FileForRead.txt";
            _fileForWritePath = projectPath + @"\FileForWrite.txt";
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

        public async Task Write()
        {
            EqSolver solver = new EqSolver();
            List<Task<string>> tasks = new List<Task<string>>();
            try
            {
                if (!_isCreated)
                {
                   await CreateFileForRead();
                    _isCreated = true;
                }
                var readFile = await File.ReadAllLinesAsync(_fileForReadPath);

                for (var index = 0; index < readFile.Length; index++)
                {
                    var index1 = index;
                    tasks.Add(Task.Run(() =>
                    {
                    var equationLine = readFile[index1];
                    var eqStrings = equationLine.Split();
                    if (double.TryParse(eqStrings[0], out double aResult) &&
                        double.TryParse(eqStrings[1], out double bResult) &&
                        double.TryParse(eqStrings[2], out double cResult))
                    {
                        var eq = new Equation() { A = aResult, B = bResult, C = cResult };
                        Console.WriteLine(index1+1);
                        return solver.ResolveEquation(eq).Result.Explanation;
                    }
                    return "";
                    }));
                }
                await File.WriteAllLinesAsync(_fileForWritePath, tasks.Select(t => t.Result));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
