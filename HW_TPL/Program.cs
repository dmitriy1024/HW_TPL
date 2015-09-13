using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HW_TPL
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();            
            InitFolders();
            stopWatch.Start();
            ShowFiles(@".", "ipsum");
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            
            Stopwatch stopWatch2 = new Stopwatch();
            InitFolders();
            stopWatch2.Start();
            ShowFilesParallel(@".", "ipsum");
            stopWatch2.Stop();
            
            TimeSpan ts2 = stopWatch2.Elapsed;

            Console.WriteLine("consistent execution {0}", ts);
            Console.WriteLine("parallel execution   {0}", ts2);
            Console.ReadKey();
        }

        static void InitFolders()
        {
            var currentDirectory = new DirectoryInfo(@".");

            for (int i = 0; i < 5; i++)
            {
                currentDirectory.CreateSubdirectory("folder" + i);
            }

            DirectoryInfo[] subDirectories = currentDirectory.GetDirectories();
            for (int i = 0; i < subDirectories.Length; i++)
            {
                var filestream = File.Create(String.Format(@"{0}\file{1}.txt", subDirectories[i].FullName, i));
                var writer = new StreamWriter(filestream);

                writer.WriteLine("Lorem ipsum dolor sit amet");
                writer.WriteLine("Lorem ipsum dolor sit amet");

                writer.Close();

                for (int j = 0; j < 5; j++)
                {
                    subDirectories[i].CreateSubdirectory(String.Format(@"folder{0}-{1}", i, j));

                    var filestream2 = File.Create(String.Format(@"{0}\file{1}-{2}.txt", subDirectories[i].GetDirectories()[j].FullName, i, j));
                    var writer2 = new StreamWriter(filestream2);

                    writer2.WriteLine("Lorem ipsum dolor sit amet");
                    writer2.WriteLine("Lorem ipsum dolor sit amet");

                    writer2.Close();
                }
            }
        }

        static void ShowFiles(string path, string findSubStr)
        {

            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles("*.txt");

            foreach (var item in files)
            {
                string line;
                var reader = item.OpenText();

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(findSubStr))
                    {
                        Console.WriteLine(item.FullName);
                        break;
                    }
                }

                reader.Close();
            }

            var subDirectories = directory.GetDirectories();
            if (subDirectories.Length == 0)
            {
                return;
            }
            else
            {
                foreach (var item in subDirectories)
                {
                    ShowFiles(item.FullName, findSubStr);
                }
            }
        }

        static void ShowFilesParallel(string path, string findSubStr)
        {

            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles("*.txt");

            foreach (var item in files)
            {
                string line;
                var reader = item.OpenText();

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(findSubStr))
                    {
                        Console.WriteLine(item.FullName);
                        break;
                    }
                }

                reader.Close();
            }

            var subDirectories = directory.GetDirectories();
            if (subDirectories.Length == 0)
            {
                return;
            }
            else
            {
                Parallel.ForEach(subDirectories, i =>
                    ShowFilesParallel(i.FullName, findSubStr)
                );
            }
        }
    }
}
